import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, timeout } from 'rxjs';
import { Page, WorkItem, WorkItemStatus } from './work-item.model';
@Injectable({ providedIn: 'root' })
export class WorkItemsService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://127.0.0.1:5062/api/work-items';
  list(title: string, status: WorkItemStatus | '', page: number): Observable<Page<WorkItem>> {
    let params = new HttpParams().set('page', page).set('pageSize', 8);
    if (title) params = params.set('title', title);
    if (status) params = params.set('status', status);
    return this.http.get<Page<WorkItem>>(this.apiUrl, { params }).pipe(timeout(10000));
  }
  create(title: string, description: string): Observable<WorkItem> { return this.http.post<WorkItem>(this.apiUrl, { title, description: description || null }).pipe(timeout(10000)); }
  updateStatus(itemId: number, status: WorkItemStatus): Observable<WorkItem> {
    return this.http.patch<WorkItem>(`${this.apiUrl}/${itemId}/status`, { status }).pipe(timeout(10000));
  }
}
