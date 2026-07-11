import { Component, OnInit, OnDestroy } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { FormsModule } from '@angular/forms';

@Component({
  standalone: true,
  selector: 'app-reservation',
  imports: [FormsModule],
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
