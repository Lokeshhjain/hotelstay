import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface HotelOfferViewModel {
  id: string;
  provider: string;
  roomType: string;
  perNightRate: number;
  totalStayPrice: number;
  cancellationPolicy: string;
  cancellationWindowHoursBeforeCheckIn: number;
  [key: string]: unknown;
}

export interface SearchCriteriaState {
  destination: string;
  checkIn: string;
  checkOut: string;
  roomType?: string;
}

export interface ReservationStateSummary {
  reservationReference?: string;
  provider?: string;
  totalPrice?: number;
  cancellationPolicy?: string;
  cancellationWindowHoursBeforeCheckIn?: number;
}

@Injectable({ providedIn: 'root' })
export class HotelStateService {
  private readonly searchResultsSubject = new BehaviorSubject<HotelOfferViewModel[]>([]);
  private readonly selectedOfferSubject = new BehaviorSubject<HotelOfferViewModel | null>(null);
  private readonly reservationStateSubject = new BehaviorSubject<{ message: string; isSuccess: boolean; summary?: ReservationStateSummary }>({ message: '', isSuccess: false });
  private readonly searchCriteriaSubject = new BehaviorSubject<SearchCriteriaState>({ destination: '', checkIn: '', checkOut: '', roomType: '' });

  readonly searchResults$ = this.searchResultsSubject.asObservable();
  readonly selectedOffer$ = this.selectedOfferSubject.asObservable();
  readonly reservationState$ = this.reservationStateSubject.asObservable();
  readonly searchCriteria$ = this.searchCriteriaSubject.asObservable();

  setSearchResults(results: HotelOfferViewModel[]): void {
    this.searchResultsSubject.next(results);
  }

  selectOffer(offer: HotelOfferViewModel | null): void {
    this.selectedOfferSubject.next(offer);
  }

  setSearchCriteria(criteria: SearchCriteriaState): void {
    this.searchCriteriaSubject.next(criteria);
  }

  setReservationState(message: string, isSuccess: boolean, summary?: ReservationStateSummary): void {
    this.reservationStateSubject.next({ message, isSuccess, summary });
  }

  clearSelection(): void {
    this.selectedOfferSubject.next(null);
  }
}
