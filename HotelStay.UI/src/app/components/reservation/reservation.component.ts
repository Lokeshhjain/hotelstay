import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../services/api.service';
import { FormsModule } from '@angular/forms';

@Component({
  standalone: true,
  selector: 'app-reservation',
  imports: [CommonModule, FormsModule],
  templateUrl: './reservation.component.html',
  styleUrls: ['./reservation.component.css']
})
export class ReservationComponent implements OnInit, OnDestroy {
  travellerName = '';
  destination = '';
  documentType = 'Passport';
  documentNumber = '';
  selectedOfferId = '';
  message = '';
  submitted = false;

  private offerSelectedHandler = (e: any) => {
    const id = e.detail?.offerId as string | undefined;
    const offer = e.detail?.offer;
    if (id) this.selectedOfferId = id;
    if (offer) {
      this.destination = offer.destination || offer.Destination || this.destination;
    }
  };

  constructor(private api: ApiService) {}

  ngOnInit() {
    window.addEventListener('offerSelected', this.offerSelectedHandler as EventListener);
  }

  ngOnDestroy() {
    window.removeEventListener('offerSelected', this.offerSelectedHandler as EventListener);
  }

  submit() {
    this.submitted = true;
    this.message = '';

    if (!this.travellerName.trim()
      || !this.destination.trim()
      || !this.documentType.trim()
      || !this.documentNumber.trim()
      || !this.selectedOfferId.trim()) {
      return;
    }

    const payload = {
      travellerName: this.travellerName,
      destination: this.destination,
      documentType: this.documentType,
      documentNumber: this.documentNumber,
      selectedOfferId: this.selectedOfferId
    };

    this.api.reserve(payload).subscribe({
      next: (res: any) => {
        this.message = `Reservation confirmed: ${res.reservationReference}`;
        window.dispatchEvent(new CustomEvent('offerReserved', { detail: { offerId: this.selectedOfferId } }));
      },
      error: (err) => {
        this.message = err?.error?.error || 'Reservation failed';
      }
    });
  }

}
