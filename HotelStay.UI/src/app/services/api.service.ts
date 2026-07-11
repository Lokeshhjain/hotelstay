import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private base = environment.apiBaseUrl || '';

  constructor(private http: HttpClient) {}

  search(destination: string, checkIn: string, checkOut: string, roomType?: string): Observable<any> {
    const params: any = { destination, checkIn, checkOut };
    if (roomType) params.roomType = roomType;
    return this.http.get(`${this.base}/hotels/search`, { params });
  }

  reserve(payload: any): Observable<any> {
    return this.http.post(`${this.base}/hotels/reserve`, payload);
  }

  lookup(reference: string): Observable<any> {
    return this.http.get(`${this.base}/hotels/reservation/${reference}`);
  }
}
