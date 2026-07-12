import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../services/api.service';
import { FormsModule } from '@angular/forms';

@Component({
  standalone: true,
  selector: 'app-search',
  imports: [CommonModule, FormsModule],
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent {
  destination = '';
  checkIn = '';
  checkOut = '';
  roomType = '';
  submitted = false;
  message = '';

  constructor(private api: ApiService) {}

  get today(): string {
    return new Date().toISOString().split('T')[0];
  }

  get checkInInvalid(): boolean {
    return !!this.checkIn && this.checkIn < this.today;
  }

  get checkOutInvalid(): boolean {
    if (!this.checkOut) return false;
    if (this.checkOut < this.today) return true;
    return !!this.checkIn && this.checkOut <= this.checkIn;
  }

  onSearch() {
    this.submitted = true;
    this.message = '';

    if (!this.destination.trim() || !this.checkIn || !this.checkOut || this.checkInInvalid || this.checkOutInvalid) {
      return;
    }

    this.api.search(this.destination, this.checkIn, this.checkOut, this.roomType).subscribe({
      next: (res: any) => {
        window.dispatchEvent(new CustomEvent('searchResults', { detail: { results: res.results || res, criteria: { destination: this.destination } } }));
      },
      error: (err) => {
        this.message = err?.error?.message || err?.error?.error || 'Search failed';
      }
    });
  }
}
