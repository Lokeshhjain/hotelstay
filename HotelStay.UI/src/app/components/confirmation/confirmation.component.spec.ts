import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BehaviorSubject } from 'rxjs';
import { ConfirmationComponent } from './confirmation.component';
import { HotelStateService } from '../../services/hotel-state.service';

describe('ConfirmationComponent', () => {
  let fixture: ComponentFixture<ConfirmationComponent>;

  beforeEach(async () => {
    const stateSpy = jasmine.createSpyObj<HotelStateService>('HotelStateService', [], {
      reservationState$: new BehaviorSubject({
        message: 'Reservation confirmed: RSV-123',
        isSuccess: true,
        summary: {
          reservationReference: 'RSV-123',
          provider: 'PremierStays',
          totalPrice: 240,
          cancellationPolicy: 'FreeCancellation',
          cancellationWindowHoursBeforeCheckIn: 48
        }
      })
    });

    await TestBed.configureTestingModule({
      imports: [ConfirmationComponent],
      providers: [{ provide: HotelStateService, useValue: stateSpy }]
    }).compileComponents();

    fixture = TestBed.createComponent(ConfirmationComponent);
  });

  it('renders the reservation summary for a successful confirmation', () => {
    fixture.detectChanges();
    const element = fixture.nativeElement as HTMLElement;

    expect(element.textContent).toContain('RSV-123');
    expect(element.textContent).toContain('PremierStays');
    expect(element.textContent).toContain('240');
  });
});
