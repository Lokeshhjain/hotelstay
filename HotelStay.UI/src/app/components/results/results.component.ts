import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HotelStateService } from '../../services/hotel-state.service';

@Component({
  standalone: true,
  selector: 'app-results',
  imports: [CommonModule],
  templateUrl: './results.component.html',
  styleUrls: ['./results.component.css']
})
export class ResultsComponent implements OnInit {
  results: any[] = [];
  selectedId = '';
  isEmpty = true;
  sortOrder: 'asc' | 'desc' = 'asc';

  constructor(private readonly state: HotelStateService) {}

  ngOnInit() {
    this.state.searchResults$.subscribe((results) => {
      this.results = this.sortResults([...results]);
      this.isEmpty = this.results.length === 0;
    });

    this.state.selectedOffer$.subscribe((offer) => {
      this.selectedId = offer?.id || '';
    });
  }

  selectOffer(id: string, offer: any): void {
    this.selectedId = id;
    this.state.selectOffer(offer);
    this.state.setReservationState('', false);
  }

  toggleSort(): void {
    this.sortOrder = this.sortOrder === 'asc' ? 'desc' : 'asc';
    this.results = this.sortResults(this.results);
  }

  private sortResults(results: any[]): any[] {
    return [...results].sort((a, b) => {
      const diff = this.getTotalPrice(a) - this.getTotalPrice(b);
      return this.sortOrder === 'asc' ? diff : -diff;
    });
  }

  private getTotalPrice(offer: any): number {
    return Number(offer.totalStayPrice ?? offer.TotalStayPrice ?? 0);
  }

  formatCancellationPolicy(offer: any): string {
    const policy = offer.cancellationPolicy ?? offer.CancellationPolicy ?? '';
    const window = offer.cancellationWindowHoursBeforeCheckIn ?? offer.CancellationWindowHoursBeforeCheckIn;

    if (window === 0) {
      return policy;
    }

    if (window) {
      return `${policy} (${window}h before check-in)`;
    }

    return policy;
  }
}
