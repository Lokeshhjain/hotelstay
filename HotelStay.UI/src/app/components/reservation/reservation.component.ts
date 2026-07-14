import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { HotelStateService } from '../../services/hotel-state.service';
import { DocumentValidationService, DocumentType } from '../../services/document-validation.service';

@Component({
  standalone: true,
  selector: 'app-reservation',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './reservation.component.html',
  styleUrls: ['./reservation.component.css']
})
export class ReservationComponent implements OnInit {
  readonly form: FormGroup;
  message = '';
  submitted = false;
  isSubmitting = false;
  documentHint = '';
  validationMessage = '';
  requiredDocumentType: DocumentType | null = null;
  availableDocumentTypes: DocumentType[] = ['NationalId', 'Passport'];

  constructor(
    private readonly api: ApiService,
    private readonly state: HotelStateService,
    private readonly fb: FormBuilder,
    private readonly documentValidator: DocumentValidationService
  ) {
    this.form = this.fb.group({
      travellerName: ['', Validators.required],
      destination: ['', Validators.required],
      documentType: ['Passport', Validators.required],
      documentNumber: ['', Validators.required],
      selectedOfferId: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.state.selectedOffer$.subscribe((offer) => {
      if (offer) {
        this.form.patchValue({
          selectedOfferId: offer.id,
          destination: this.form.get('destination')?.value || ''
        });
      }
    });

    this.state.searchCriteria$.subscribe((criteria) => {
      if (criteria.destination) {
        this.form.patchValue({ destination: criteria.destination });
        this.updateDocumentRequirement(criteria.destination);
      }
    });
  }

  /**
   * Updates the document requirement hint and sets the appropriate document type
   * based on the destination category.
   */
  updateDocumentRequirement(destination: string): void {
    if (!destination || destination.trim() === '') {
      this.documentHint = '';
      this.requiredDocumentType = null;
      this.availableDocumentTypes = ['NationalId', 'Passport'];
      return;
    }

    const category = this.documentValidator.tryDetermineCategoryByDestination(destination);
    
    if (!category) {
      // Unknown destination
      this.documentHint = '';
      this.requiredDocumentType = null;
      this.availableDocumentTypes = ['NationalId', 'Passport'];
      return;
    }

    const required = category === 'Domestic' ? 'NationalId' : 'Passport';
    
    this.requiredDocumentType = required;
    this.availableDocumentTypes = [required]; // Restrict to only the valid option
    this.documentHint = `${category} destinations require a ${this.documentValidator.getDocumentTypeDisplay(required)}.`;
    
    // Auto-select the required document type
    this.form.patchValue({ documentType: required });
  }

  /**
   * Get display name for a document type.
   */
  getDocumentTypeDisplay(docType: DocumentType): string {
    return this.documentValidator.getDocumentTypeDisplay(docType);
  }

  submit(): void {
    this.submitted = true;
    this.message = '';
    this.validationMessage = '';

    // Check if form has basic validation errors
    if (this.form.invalid) {
      this.validationMessage = 'Please complete the required reservation details before submitting.';
      return;
    }

    const destination = this.form.get('destination')?.value?.trim() || '';
    const documentType = this.form.get('documentType')?.value;
    const documentNumber = this.form.get('documentNumber')?.value?.trim() || '';

    // Validate document against destination
    const validation = this.documentValidator.validateDocument(
      destination,
      documentType,
      documentNumber
    );

    if (!validation.isValid) {
      this.validationMessage = validation.message;
      return;
    }

    // Submit the reservation
    this.isSubmitting = true;
    const payload = this.form.value;

    this.api.reserve(payload).subscribe({
      next: (res: any) => {
        this.isSubmitting = false;
        const confirmation = `Reservation confirmed: ${res.reservationReference}`;
        this.message = confirmation;
        this.state.setReservationState(confirmation, true, {
          reservationReference: res.reservationReference,
          provider: res.provider,
          totalPrice: res.totalPrice,
          cancellationPolicy: res.cancellationPolicy,
          cancellationWindowHoursBeforeCheckIn: res.offerSnapshot?.cancellationWindowHoursBeforeCheckIn
        });
        this.state.clearSelection();
      },
      error: (err) => {
        this.isSubmitting = false;
        // Server validation may return 422 with document validation error
        const serverMessage = err?.error?.error || 'Reservation failed';
        this.message = serverMessage;
        this.validationMessage = serverMessage;
        this.state.setReservationState(serverMessage, false);
      }
    });
  }
}

