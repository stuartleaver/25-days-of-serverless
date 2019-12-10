import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class BestdealsService {
  constructor(private http: HttpClient) { }

  getBestDeals() {
    return this.http.get('https://twitterbestdeals.azurewebsites.net/api/GetBestDeals');
  }
}
