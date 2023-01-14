import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { retry, catchError } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { DeviceDetail } from 'models/devicedetail';

@Injectable({
  providedIn: 'root'
})
export class DeviceDetailService {

  myAppUrl: string;
  myApiUrl: string;
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json; charset=utf-8'
    })
  };
  constructor(private http: HttpClient) {
    this.myAppUrl = environment.appUrl;
    this.myApiUrl = 'api/DeviceDetails/';
  }

  getDeviceDetails(): Observable<DeviceDetail[]> {
    return this.http.get<DeviceDetail[]>(this.myAppUrl + this.myApiUrl)
      .pipe(
        retry(1),
        catchError(this.errorHandler)
      );
  }

  getDeviceDetail(deviceId: string, serverDate: any): Observable<DeviceDetail> {
    if (serverDate == null || serverDate == undefined) {
      return this.http.get<DeviceDetail>(this.myAppUrl + this.myApiUrl + deviceId)
        .pipe(
          retry(1),
          catchError(this.errorHandler)
        );
    } else {
      return this.http.get<DeviceDetail>(this.myAppUrl + this.myApiUrl + deviceId + "/" + serverDate)
        .pipe(
          retry(1),
          catchError(this.errorHandler)
        );

    }
  }

  saveDeviceDetail(deviceDetail: any): Observable<DeviceDetail> {
    return this.http.post<DeviceDetail>(this.myAppUrl + this.myApiUrl, JSON.stringify(deviceDetail), this.httpOptions)
      .pipe(
        retry(1),
        catchError(this.errorHandler)
      );
  }

  updateDeviceDetail(deviceId: string, deviceDetail: any): Observable<DeviceDetail> {
    return this.http.put<DeviceDetail>(this.myAppUrl + this.myApiUrl + deviceId, JSON.stringify(deviceDetail), this.httpOptions)
      .pipe(
        retry(1),
        catchError(this.errorHandler)
      );
  }

  deleteDeviceDetail(deviceId: string): Observable<DeviceDetail> {
    return this.http.delete<DeviceDetail>(this.myAppUrl + this.myApiUrl + deviceId)
      .pipe(
        retry(1),
        catchError(this.errorHandler)
      );
  }

  errorHandler(error: any) {
    let errorMessage = '';
    if (error.error instanceof ErrorEvent) {
      // Get client-side error
      errorMessage = error.error.message;
    } else {
      // Get server-side error
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    console.log(errorMessage);
    return throwError(errorMessage);
  }
}
