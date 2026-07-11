import { Component } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { FormsModule } from '@angular/forms';

@Component({
  standalone: true,
  selector: 'app-search',
  imports: [FormsModule],
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent {
  destination = '';
  checkIn = '';
  checkOut = '';
  roomType = '';

  constructor(private api: ApiService) {}

  onSearch() {
    this.api.search(this.destination, this.checkIn, this.checkOut, this.roomType).subscribe({
      next: (res: any) => {
        window.dispatchEvent(new CustomEvent('searchResults', { detail: { results: res.results || res, criteria: { destination: this.destination } } }));
      },
      error: (err) => console.error(err)
    });
  }
}
