import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { BehaviorSubject, catchError, debounceTime, distinctUntilChanged, finalize, map, merge, of, startWith, Subject, switchMap, tap } from 'rxjs';
import { WorkItemStatus } from './work-item.model';
import { WorkItemsService } from './work-items.service';

@Component({ selector: 'app-root', standalone: true, imports: [CommonModule, ReactiveFormsModule], templateUrl: './app.component.html', changeDetection: ChangeDetectionStrategy.OnPush })
export class AppComponent {
  private readonly service = inject(WorkItemsService);
  private readonly refresh$ = new Subject<void>();
  readonly page$ = new BehaviorSubject(1);
  readonly statuses: { value: WorkItemStatus | ''; label: string }[] = [{ value: '', label: 'All statuses' }, { value: 'Todo', label: 'To do' }, { value: 'InProgress', label: 'In progress' }, { value: 'Done', label: 'Done' }];
  readonly createForm = new FormGroup({ title: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(120)] }), description: new FormControl('', { nonNullable: true }) });
  readonly search = new FormControl('', { nonNullable: true });
  readonly status = new FormControl<WorkItemStatus | ''>('', { nonNullable: true });
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal('');

  readonly result$ = merge(
    this.search.valueChanges.pipe(debounceTime(300), distinctUntilChanged(), tap(() => this.page$.next(1))),
    this.status.valueChanges.pipe(tap(() => this.page$.next(1))),
    this.page$,
    this.refresh$
  ).pipe(startWith(null), tap(() => { this.loading.set(true); this.error.set(''); }),
    switchMap(() => this.service.list(this.search.value.trim(), this.status.value, this.page$.value).pipe(
      map(page => ({ page, failed: false })), catchError(() => of({ page: null, failed: true })), finalize(() => this.loading.set(false)))),
    tap(result => { if (result.failed) this.error.set('Could not load work items. Please try again.'); }));

  create(): void {
    if (this.createForm.invalid) { this.createForm.markAllAsTouched(); return; }
    this.saving.set(true); this.error.set('');
    const { title, description } = this.createForm.getRawValue();
    this.service.create(title, description).pipe(finalize(() => this.saving.set(false))).subscribe({ next: () => { this.createForm.reset(); this.page$.next(1); this.refresh$.next(); }, error: error => this.error.set(this.describeError('create', error)) });
  }
  changeStatus(itemId: number, currentStatus: WorkItemStatus, event: Event): void {
    const select = event.target as HTMLSelectElement;
    const nextStatus = select.value as WorkItemStatus;
    if (nextStatus === currentStatus) return;
    this.error.set('');
    this.service.updateStatus(itemId, nextStatus).subscribe({
      next: () => this.refresh$.next(),
      error: error => {
        select.value = currentStatus;
        this.error.set(this.describeError('update', error));
      }
    });
  }
  previous(): void { if (this.page$.value > 1) this.page$.next(this.page$.value - 1); }
  next(totalPages: number): void { if (this.page$.value < totalPages) this.page$.next(this.page$.value + 1); }
  retry(): void { this.refresh$.next(); }
  label(status: WorkItemStatus): string { return status === 'InProgress' ? 'In progress' : status === 'Todo' ? 'To do' : 'Done'; }
  statusChoices(status: WorkItemStatus): WorkItemStatus[] {
    if (status === 'Todo') return ['Todo', 'InProgress'];
    if (status === 'InProgress') return ['InProgress', 'Done'];
    return ['Done'];
  }

  private describeError(action: string, error: unknown): string {
    if (error instanceof HttpErrorResponse) {
      if (error.status === 0) return `Could not ${action} the work item: API is unreachable or blocked by CORS.`;
      const detail = typeof error.error?.detail === 'string' ? ` ${error.error.detail}` : '';
      if (typeof error.error === 'string' && error.error.trim()) {
        return `Could not ${action} the work item: ${error.error.split('\n')[0]}`;
      }
      return `Could not ${action} the work item: HTTP ${error.status} ${error.statusText}.${detail}`;
    }
    return `Could not ${action} the work item: request timed out.`;
  }
}
