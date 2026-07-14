import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { of, throwError, BehaviorSubject } from 'rxjs';
import { ReservationComponent } from './reservation.component';
import { ApiService } from '../../services/api.service';
import { HotelStateService } from '../../services/hotel-state.service';
import { DocumentValidationService } from '../../services/document-validation.service';

describe('ReservationComponent', () => {
  let fixture: ComponentFixture<ReservationComponent>;
  let component: ReservationComponent;
  let apiSpy: jasmine.SpyObj<ApiService>;
  let stateSpy: jasmine.SpyObj<Partial<HotelStateService>> & { selectedOffer$: BehaviorSubject<any> };
  let validatorSpy: jasmine.SpyObj<DocumentValidationService>;

  beforeEach(async () => {
    apiSpy = jasmine.createSpyObj('ApiService', ['reserve']);
    validatorSpy = jasmine.createSpyObj('DocumentValidationService', ['validateDocument', 'tryDetermineCategoryByDestination', 'getDocumentTypeDisplay']);

    // Create a lightweight state spy with observable properties
    stateSpy = {
      selectedOffer$: new BehaviorSubject<any>(null),
      searchCriteria$: of({ destination: '', checkIn: '', checkOut: '', roomType: '' }),
      setReservationState: jasmine.createSpy('setReservationState'),
      removeOfferFromResults: jasmine.createSpy('removeOfferFromResults'),
      clearSelection: jasmine.createSpy('clearSelection')
    } as any;

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
    component.form.patchValue({ travellerName: '', destination: '', documentType: 'Passport', documentNumber: '' });
    component.submit();

    expect(component.validationMessage).toBe('Please complete the required reservation details before submitting.');
    expect(apiSpy.reserve).not.toHaveBeenCalled();
  });

  it('should reject invalid document type before submitting', () => {
    component.form.patchValue({ travellerName: 'Alex', destination: 'Paris', documentType: 'NationalId', documentNumber: 'NID-123' });
    validatorSpy.validateDocument.and.returnValue({ isValid: false, message: 'International destinations require a Passport.', requiredDocumentType: 'Passport' });

    component.submit();

    expect(component.validationMessage).toBe('International destinations require a Passport.');
    expect(apiSpy.reserve).not.toHaveBeenCalled();
  });

  it('should submit reservation when document is valid and offer selected', () => {
    component.form.patchValue({ travellerName: 'Alex', destination: 'Delhi', documentType: 'NationalId', documentNumber: 'NID-123' });
    validatorSpy.validateDocument.and.returnValue({ isValid: true, message: 'Document is valid.', requiredDocumentType: 'NationalId' });
    apiSpy.reserve.and.returnValue(of({ reservationReference: 'RSV-123', provider: 'PremierStays', totalPrice: 360, cancellationPolicy: 'FreeCancellation', offerSnapshot: { cancellationWindowHoursBeforeCheckIn: 48 } }));

    // Simulate an offer being selected in state
    stateSpy.selectedOffer$.next({ id: 'offer-1', provider: 'PremierStays' });

    component.submit();

    expect(component.isSubmitting).toBeFalse();
    expect(component.message).toBe('');
    expect(component.validationMessage).toBe('');
    expect(component.form.value).toEqual({
      travellerName: '',
      destination: '',
      documentType: 'Passport',
      documentNumber: ''
    });
    expect((stateSpy as any).setReservationState).toHaveBeenCalledWith('Reservation confirmed: RSV-123', true, jasmine.any(Object));
    expect((stateSpy as any).removeOfferFromResults).toHaveBeenCalledWith('offer-1');
    expect((stateSpy as any).clearSelection).toHaveBeenCalled();
  });

  it('should handle reservation failure from API', () => {
    component.form.patchValue({ travellerName: 'Alex', destination: 'Delhi', documentType: 'NationalId', documentNumber: 'NID-123' });
    validatorSpy.validateDocument.and.returnValue({ isValid: true, message: 'Document is valid.', requiredDocumentType: 'NationalId' });
    apiSpy.reserve.and.returnValue(throwError(() => ({ error: { error: 'Reservation failed' } })));

    stateSpy.selectedOffer$.next({ id: 'offer-1', provider: 'PremierStays' });

    component.submit();

    expect(component.validationMessage).toBe('Reservation failed');
    expect(component.message).toBe('Reservation failed');
    expect((stateSpy as any).setReservationState).toHaveBeenCalledWith('Reservation failed', false);
  });
});
