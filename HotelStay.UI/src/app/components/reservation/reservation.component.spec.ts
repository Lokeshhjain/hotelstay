import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';
import { ReservationComponent } from './reservation.component';
import { ApiService } from '../../services/api.service';
import { HotelStateService } from '../../services/hotel-state.service';
import { DocumentValidationService } from '../../services/document-validation.service';

describe('ReservationComponent', () => {
  let fixture: ComponentFixture<ReservationComponent>;
  let component: ReservationComponent;
  let apiSpy: jasmine.SpyObj<ApiService>;
  let stateSpy: jasmine.SpyObj<HotelStateService>;
  let validatorSpy: jasmine.SpyObj<DocumentValidationService>;

  beforeEach(async () => {
    apiSpy = jasmine.createSpyObj('ApiService', ['reserve']);
    stateSpy = jasmine.createSpyObj('HotelStateService', ['setReservationState', 'clearSelection'], {
      selectedOffer$: of(null),
      searchCriteria$: of({ destination: '', checkIn: '', checkOut: '', roomType: '' })
    });
    validatorSpy = jasmine.createSpyObj('DocumentValidationService', ['validateDocument', 'tryDetermineCategoryByDestination', 'getDocumentTypeDisplay']);

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, ReservationComponent],
      providers: [
        { provide: ApiService, useValue: apiSpy },
        { provide: HotelStateService, useValue: stateSpy },
        { provide: DocumentValidationService, useValue: validatorSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ReservationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should show validation message when form is incomplete', () => {
    component.form.patchValue({ travellerName: '', destination: '', documentType: 'Passport', documentNumber: '', selectedOfferId: '' });
    component.submit();

    expect(component.validationMessage).toBe('Please complete the required reservation details before submitting.');
    expect(apiSpy.reserve).not.toHaveBeenCalled();
  });

  it('should reject invalid document type before submitting', () => {
    component.form.patchValue({ travellerName: 'Alex', destination: 'Paris', documentType: 'NationalId', documentNumber: 'NID-123', selectedOfferId: 'offer-1' });
    validatorSpy.validateDocument.and.returnValue({ isValid: false, message: 'International destinations require a Passport.', requiredDocumentType: 'Passport' });

    component.submit();

    expect(component.validationMessage).toBe('International destinations require a Passport.');
    expect(apiSpy.reserve).not.toHaveBeenCalled();
  });

  it('should submit reservation when document is valid', () => {
    component.form.patchValue({ travellerName: 'Alex', destination: 'Delhi', documentType: 'NationalId', documentNumber: 'NID-123', selectedOfferId: 'offer-1' });
    validatorSpy.validateDocument.and.returnValue({ isValid: true, message: 'Document is valid.', requiredDocumentType: 'NationalId' });
    apiSpy.reserve.and.returnValue(of({ reservationReference: 'RSV-123', provider: 'PremierStays', totalPrice: 360, cancellationPolicy: 'FreeCancellation', offerSnapshot: { cancellationWindowHoursBeforeCheckIn: 48 } }));

    component.submit();

    expect(component.isSubmitting).toBeFalse();
    expect(component.message).toContain('Reservation confirmed: RSV-123');
    expect(stateSpy.setReservationState).toHaveBeenCalledWith('Reservation confirmed: RSV-123', true, jasmine.any(Object));
    expect(stateSpy.clearSelection).toHaveBeenCalled();
  });

  it('should handle reservation failure from API', () => {
    component.form.patchValue({ travellerName: 'Alex', destination: 'Delhi', documentType: 'NationalId', documentNumber: 'NID-123', selectedOfferId: 'offer-1' });
    validatorSpy.validateDocument.and.returnValue({ isValid: true, message: 'Document is valid.', requiredDocumentType: 'NationalId' });
    apiSpy.reserve.and.returnValue(throwError(() => ({ error: { error: 'Reservation failed' } })));

    component.submit();

    expect(component.validationMessage).toBe('Reservation failed');
    expect(component.message).toBe('Reservation failed');
    expect(stateSpy.setReservationState).toHaveBeenCalledWith('Reservation failed', false);
  });
});
