export type WorkItemStatus = 'Todo' | 'InProgress' | 'Done';
export interface WorkItem { id: number; title: string; description: string | null; status: WorkItemStatus; createdAt: string; }
export interface Page<T> { items: T[]; page: number; pageSize: number; totalCount: number; totalPages: number; }
