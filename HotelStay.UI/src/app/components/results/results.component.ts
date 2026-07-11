import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-results',
  imports: [CommonModule],
  templateUrl: './results.component.html',
  styleUrls: ['./results.component.css']
})
export class ResultsComponent implements OnInit, OnDestroy {
  results: any[] = [];
  selectedId = '';

  private searchHandler = (e: any) => {
    this.results = e.detail?.results || [];
  };

  private reservedHandler = (e: any) => {
    const id = e.detail?.offerId as string | undefined;
    if (!id) return;
    this.results = this.results.filter(r => (r.id || r.Id) !== id);
  };

  ngOnInit() {
    window.addEventListener('searchResults', this.searchHandler as EventListener);
    window.addEventListener('offerReserved', this.reservedHandler as EventListener);
  }

  ngOnDestroy() {
    window.removeEventListener('searchResults', this.searchHandler as EventListener);
    window.removeEventListener('offerReserved', this.reservedHandler as EventListener);
  }

  selectOffer(id: string, offer: any) {
    this.selectedId = id;
    window.dispatchEvent(new CustomEvent('offerSelected', { detail: { offerId: id, offer } }));
    try {
      const el = document.querySelector('app-reservation');
      if (el && (el as HTMLElement).scrollIntoView) {
        (el as HTMLElement).scrollIntoView({ behavior: 'smooth', block: 'center' });
      }
    } catch { }
  }
}
