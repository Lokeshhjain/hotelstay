import { Component } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

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

  constructor(private api: ApiService) {}

  onLookup() {
    this.api.lookup(this.reference).subscribe({ next: (r) => this.result = r, error: () => this.result = { error: 'Not found' } });
  }
}
