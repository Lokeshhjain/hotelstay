import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BehaviorSubject } from 'rxjs';
import { ResultsComponent } from './results.component';
import { HotelStateService } from '../../services/hotel-state.service';

describe('ResultsComponent', () => {
  let fixture: ComponentFixture<ResultsComponent>;
  let component: ResultsComponent;
  let stateService: jasmine.SpyObj<HotelStateService>;

  beforeEach(async () => {
    const stateSpy = jasmine.createSpyObj<HotelStateService>('HotelStateService', ['selectOffer', 'setReservationState'], {
      searchResults$: new BehaviorSubject<any[]>([
        { id: 'b', totalStayPrice: 200 },
        { id: 'a', totalStayPrice: 100 },
        { id: 'c', totalStayPrice: 150 }
      ]),
      selectedOffer$: new BehaviorSubject<any>(null)
    });

    await TestBed.configureTestingModule({
      imports: [ResultsComponent],
      providers: [{ provide: HotelStateService, useValue: stateSpy }]
    }).compileComponents();

    fixture = TestBed.createComponent(ResultsComponent);
    component = fixture.componentInstance;
    stateService = TestBed.inject(HotelStateService) as jasmine.SpyObj<HotelStateService>;
  });

  it('sorts results by total stay price when search results arrive', () => {
    component.ngOnInit();

    expect(component.results.map((offer: any) => offer.id)).toEqual(['a', 'c', 'b']);
  });
});
