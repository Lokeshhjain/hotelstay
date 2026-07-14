import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';
import { LookupComponent } from './lookup.component';
import { ApiService } from '../../services/api.service';

describe('LookupComponent', () => {
  let fixture: ComponentFixture<LookupComponent>;
  let component: LookupComponent;
  let apiSpy: jasmine.SpyObj<ApiService>;

  beforeEach(async () => {
    apiSpy = jasmine.createSpyObj('ApiService', ['lookup']);

    await TestBed.configureTestingModule({
      imports: [FormsModule, LookupComponent],
      providers: [{ provide: ApiService, useValue: apiSpy }]
    }).compileComponents();

    fixture = TestBed.createComponent(LookupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should not attempt lookup when reference is empty', () => {
    component.reference = '';
    component.onLookup();

    expect(component.submitted).toBeTrue();
    expect(component.result).toBeNull();
    expect(apiSpy.lookup).not.toHaveBeenCalled();
  });

  it('should enable lookup only when reference contains text', () => {
    component.reference = ' ';
    expect(component.canLookup).toBeFalse();

    component.reference = 'RSV-123';
    expect(component.canLookup).toBeTrue();
  });

  it('should set result when lookup succeeds', () => {
    apiSpy.lookup.and.returnValue(of({ reservationReference: 'RSV-123', travellerName: 'Alex', provider: 'PremierStays', roomType: 'Standard', totalPrice: 360, cancellationPolicy: 'FreeCancellation', validationOutcome: 'Accepted' }));
    component.reference = 'RSV-123';

    component.onLookup();

    expect(apiSpy.lookup).toHaveBeenCalledWith('RSV-123');
    expect(component.result).toEqual(jasmine.objectContaining({ reservationReference: 'RSV-123' }));
    expect(component.submitted).toBeTrue();
  });

  it('should set error result when lookup fails', () => {
    apiSpy.lookup.and.returnValue(throwError(() => ({ error: { message: 'Not found' } })));
    component.reference = 'RSV-404';

    component.onLookup();

    expect(apiSpy.lookup).toHaveBeenCalledWith('RSV-404');
    expect(component.result).toEqual({ error: 'Not found' });
    expect(component.submitted).toBeTrue();
  });
});
