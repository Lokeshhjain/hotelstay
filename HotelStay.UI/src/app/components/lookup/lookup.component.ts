import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../services/api.service';
import { FormsModule } from '@angular/forms';

@Component({
  standalone: true,
  selector: 'app-lookup',
  imports: [CommonModule, FormsModule],
  templateUrl: './lookup.component.html',
  styleUrls: ['./lookup.component.css']
})
export class LookupComponent {
  reference = '';
  result: any = null;
  submitted = false;
  message = '';

  constructor(private api: ApiService) {}

  onLookup() {
    this.submitted = true;
    this.message = '';
    this.result = null;

    if (!this.reference.trim()) {
      return;
    }

    this.api.lookup(this.reference).subscribe({
      next: (r) => this.result = r,
      error: () => this.result = { error: 'Not found' }
    });
  }

  get canLookup(): boolean {
    return !!this.reference.trim();
  }
}
