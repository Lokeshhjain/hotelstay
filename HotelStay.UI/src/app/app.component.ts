import { Component } from '@angular/core';
import { SearchComponent } from './components/search/search.component';
import { ResultsComponent } from './components/results/results.component';
import { ReservationComponent } from './components/reservation/reservation.component';
import { LookupComponent } from './components/lookup/lookup.component';
import { ConfirmationComponent } from './components/confirmation/confirmation.component';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [SearchComponent, ResultsComponent, ReservationComponent, LookupComponent, ConfirmationComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'SkyRoute Hotel Availability';
}
