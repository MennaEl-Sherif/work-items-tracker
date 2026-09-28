import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { WorkItemsService } from './work-items.service';

describe('WorkItemsService', () => {
  let service: WorkItemsService; let http: HttpTestingController;
  beforeEach(() => { TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting()] }); service = TestBed.inject(WorkItemsService); http = TestBed.inject(HttpTestingController); });
  afterEach(() => http.verify());
  it('sends filters and pagination', () => {
    service.list('report', 'Todo', 2).subscribe();
    const request = http.expectOne(r => r.url === '/api/work-items');
    expect(request.request.params.get('title')).toBe('report'); expect(request.request.params.get('status')).toBe('Todo'); expect(request.request.params.get('page')).toBe('2');
    request.flush({ items: [], page: 2, pageSize: 8, totalCount: 0, totalPages: 0 });
  });
  it('updates an item status', () => {
    service.updateStatus(7, 'InProgress').subscribe();
    const request = http.expectOne('/api/work-items/7/status'); expect(request.request.method).toBe('PATCH'); expect(request.request.body).toEqual({ status: 'InProgress' }); request.flush({});
  });
});
