import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';
import { SearchComponent } from './search.component';
import { ApiService } from '../../services/api.service';
import { HotelStateService } from '../../services/hotel-state.service';

describe('SearchComponent', () => {
  let fixture: ComponentFixture<SearchComponent>;
  let component: SearchComponent;
  let apiSpy: jasmine.SpyObj<ApiService>;
  let stateSpy: jasmine.SpyObj<HotelStateService>;

  beforeEach(async () => {
    apiSpy = jasmine.createSpyObj('ApiService', ['search']);
    stateSpy = jasmine.createSpyObj('HotelStateService', ['setSearchCriteria', 'setSearchResults'], {
      searchCriteria$: of({ destination: '', checkIn: '', checkOut: '', roomType: '' }),
      searchResults$: of([])
    });

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, SearchComponent],
      providers: [
        { provide: ApiService, useValue: apiSpy },
        { provide: HotelStateService, useValue: stateSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(SearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should disable submit when required fields are missing', () => {
    component.form.patchValue({ destination: '', checkIn: '', checkOut: '' });

    component.onSearch();

    expect(component.submitted).toBeTrue();
    expect(apiSpy.search).not.toHaveBeenCalled();
  });

  it('should call ApiService.search and update state on successful search', () => {
    const today = new Date(component.today);
    const checkIn = new Date(today);
    checkIn.setDate(today.getDate() + 1);
    const checkOut = new Date(today);
    checkOut.setDate(today.getDate() + 4);

    const checkInValue = checkIn.toISOString().split('T')[0];
    const checkOutValue = checkOut.toISOString().split('T')[0];

    apiSpy.search.and.returnValue(of({ results: [{ id: 'offer-1', totalStayPrice: 300 }] }));

    component.form.patchValue({ destination: 'Delhi', checkIn: checkInValue, checkOut: checkOutValue, roomType: 'standard' });
    component.onSearch();

    expect(stateSpy.setSearchCriteria).toHaveBeenCalledWith({
      destination: 'Delhi',
      checkIn: checkInValue,
      checkOut: checkOutValue,
      roomType: 'standard'
    });
    expect(apiSpy.search).toHaveBeenCalledWith('Delhi', checkInValue, checkOutValue, 'standard');
    expect(stateSpy.setSearchResults).toHaveBeenCalledWith(jasmine.arrayContaining([
      jasmine.objectContaining({ id: 'offer-1', totalStayPrice: 300 })
    ]));
    expect(component.isLoading).toBeFalse();
  });

  it('should display server error message when search fails', () => {
    const today = new Date(component.today);
    const checkIn = new Date(today);
    checkIn.setDate(today.getDate() + 1);
    const checkOut = new Date(today);
    checkOut.setDate(today.getDate() + 4);

    const checkInValue = checkIn.toISOString().split('T')[0];
    const checkOutValue = checkOut.toISOString().split('T')[0];

    apiSpy.search.and.returnValue(throwError(() => ({ error: { message: 'Search failed' } })));

    component.form.patchValue({ destination: 'Delhi', checkIn: checkInValue, checkOut: checkOutValue });
    component.onSearch();

    expect(component.message).toBe('Search failed');
    expect(stateSpy.setSearchResults).toHaveBeenCalledWith([]);
  });
});
