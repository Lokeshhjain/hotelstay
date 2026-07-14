import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HotelStateService } from '../../services/hotel-state.service';

@Component({
  standalone: true,
  selector: 'app-confirmation',
  imports: [CommonModule],
  templateUrl: './confirmation.component.html',
  styleUrls: ['./confirmation.component.css']
})
export class ConfirmationComponent implements OnInit {
  reservationReference = '';
  provider = '';
  totalPrice = '';
  cancellationPolicy = '';
  cancellationWindowHoursBeforeCheckIn: number | undefined;
  message = '';
  isSuccess = false;
  isVisible = false;

  constructor(private readonly state: HotelStateService) {}

  ngOnInit(): void {
    this.state.reservationState$.subscribe((reservationState) => {
      this.message = reservationState.message;
      this.isSuccess = reservationState.isSuccess;
      this.isVisible = !!reservationState.message;

      if (reservationState.summary) {
        this.reservationReference = reservationState.summary.reservationReference || '';
        this.provider = reservationState.summary.provider || '';
        this.totalPrice = reservationState.summary.totalPrice?.toString() || '';
        this.cancellationPolicy = reservationState.summary.cancellationPolicy || '';
        this.cancellationWindowHoursBeforeCheckIn = reservationState.summary.cancellationWindowHoursBeforeCheckIn;
      } else {
        this.reservationReference = '';
        this.provider = '';
        this.totalPrice = '';
        this.cancellationPolicy = '';
        this.cancellationWindowHoursBeforeCheckIn = undefined;
      }
    });
  }

  formatCancellationPolicy(): string {
    if (!this.cancellationPolicy) {
      return '';
    }

    if (this.cancellationWindowHoursBeforeCheckIn === 0) {
      return this.cancellationPolicy;
    }

    if (this.cancellationWindowHoursBeforeCheckIn) {
      return `${this.cancellationPolicy} (${this.cancellationWindowHoursBeforeCheckIn}h before check-in)`;
    }

    return this.cancellationPolicy;
  }
}
