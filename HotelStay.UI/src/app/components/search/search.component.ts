import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { HotelStateService } from '../../services/hotel-state.service';

@Component({
  standalone: true,
  selector: 'app-search',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent {
  readonly form: FormGroup;
  submitted = false;
  message = '';
  isLoading = false;

  constructor(
    private readonly api: ApiService,
    private readonly state: HotelStateService,
    private readonly fb: FormBuilder
  ) {
    this.form = this.fb.group({
      destination: ['', Validators.required],
      checkIn: ['', Validators.required],
      checkOut: ['', Validators.required],
      roomType: ['']
    });
  }

  get today(): string {
    return new Date().toISOString().split('T')[0];
  }

  get checkInInvalid(): boolean {
    const checkIn = this.form.get('checkIn')?.value;
    return !!checkIn && checkIn < this.today;
  }

  get checkOutInvalid(): boolean {
    const checkIn = this.form.get('checkIn')?.value;
    const checkOut = this.form.get('checkOut')?.value;
    if (!checkOut) return false;
    if (checkOut < this.today) return true;
    return !!checkIn && checkOut <= checkIn;
  }

  onSearch(): void {
    this.submitted = true;
    this.message = '';

    if (this.form.invalid || this.checkInInvalid || this.checkOutInvalid) {
      return;
    }

    const values = this.form.value;
    this.isLoading = true;
    this.state.setSearchCriteria({
      destination: values.destination,
      checkIn: values.checkIn,
      checkOut: values.checkOut,
      roomType: values.roomType
    });

    this.api.search(values.destination, values.checkIn, values.checkOut, values.roomType).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        const results = (res.results || res || []) as any[];
        this.state.setSearchResults(results);
      },
      error: (err) => {
        this.isLoading = false;
        this.message = err?.error?.message || err?.error?.error || 'Search failed';
        this.state.setSearchResults([]);
      }
    });
  }
}
