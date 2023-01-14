import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { retry, catchError } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { DeviceDynamicProperty } from 'models/devicedynamicproperty';

@Injectable({
  providedIn: 'root'
})
export class DeviceDynamicPropertyService {

  myAppUrl: string;
  myApiUrl: string;
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json; charset=utf-8'
    })
  };
  constructor(private http: HttpClient) {
      this.myAppUrl = environment.appUrl;
      this.myApiUrl = 'api/DeviceDynamicProperties/';
  }

  getDeviceDynamicProperties(): Observable<DeviceDynamicProperty[]> {
    return this.http.get<DeviceDynamicProperty[]>(this.myAppUrl + this.myApiUrl)
    .pipe(
      retry(1),
      catchError(this.errorHandler)
    );
  }

  getDeviceDynamicProperty(propertyId: number): Observable<DeviceDynamicProperty> {
      return this.http.get<DeviceDynamicProperty>(this.myAppUrl + this.myApiUrl + propertyId)
      .pipe(
        retry(1),
        catchError(this.errorHandler)
      );
  }

  saveDeviceDynamicProperty(deviceDynamicProperty: any): Observable<DeviceDynamicProperty> {
      return this.http.post<DeviceDynamicProperty>(this.myAppUrl + this.myApiUrl, JSON.stringify(deviceDynamicProperty), this.httpOptions)
      .pipe(
        retry(1),
        catchError(this.errorHandler)
      );
  }

  updateDeviceDynamicProperty(propertyId: number, deviceDynamicProperty:any): Observable<DeviceDynamicProperty> {
      return this.http.put<DeviceDynamicProperty>(this.myAppUrl + this.myApiUrl + propertyId, JSON.stringify(deviceDynamicProperty), this.httpOptions)
      .pipe(
        retry(1),
        catchError(this.errorHandler)
      );
  }

  deleteDeviceDynamicProperty(propertyId: number): Observable<DeviceDynamicProperty> {
      return this.http.delete<DeviceDynamicProperty>(this.myAppUrl + this.myApiUrl + propertyId)
      .pipe(
        retry(1),
        catchError(this.errorHandler)
      );
  }

  errorHandler(error:any) {
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