import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../environment';
import { ApiRequestLog } from '../models/api-request-log/api-request-log.module';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiLogServiceService {
    private apiUrl  = `${environment.apiUrl}/logs`;

  constructor(private http: HttpClient) { }

  getLogs(): Observable<ApiRequestLog[]> {
    return this.http.get<ApiRequestLog[]>(this.apiUrl);
  }
}
