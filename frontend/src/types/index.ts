
// Auth Types
export interface User {
  id: string;
  userName: string;
  email: string;
  role: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  userName: string;
  email: string;
  password: string;
}

export interface UpdateAccountRequest {
  userName?: string;
  currentPassword?: string;
  newPassword?: string;
  email?: string;
}

export interface CreateModeratorRequest {
  userName: string;
  email: string;
  password: string;
}

// Review Types
export interface Review {
  reviewId: string;
  userId: string;
  gameName: string;
  description: string;
  createdAt: string;
  tags: string[];
}

export interface CreateReviewRequest {
  gameName: string;
  description: string;
  tags: string[];
}

export interface UpdateReviewRequest {
  gameName: string;
  description: string;
  tags: string[];
}

// Comment Types
export interface Comment {
  id: string;
  userId: string;
  reviewId: string;
  parentId?: string;
  text: string;
  createdAt: string;
  userName: string;
}

export interface CreateCommentRequest {
  text: string;
}

export interface UpdateCommentRequest {
  text: string;
}

// Report Types
export enum ReportStatus {
  InProgress = 0,
  Resolved = 1,
  Dismissed = 2,
}

export enum ReportedContentType {
  Review = 0,
  Comment = 1,
}

export interface Report {
  id: string;
  reporterId: string;
  reportedUserId: string;
  contentId: string;
  contentType: ReportedContentType;
  reason: string;
  description: string;
  status: ReportStatus;
  createdAt: string;
}

export interface CreateReportRequest {
  reportedUserId: string;
  contentId: string;
  contentType: ReportedContentType;
  reason: string;
  description: string;
}

export interface UpdateReportStatusRequest {
  status: ReportStatus;
}

// Filter and Pagination Types
export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface FilterRequest {
  search?: string;
  sortBy?: string;
  sortDirection?: string;
  page?: number;
  pageSize?: number;
}

export interface ReviewFilterRequest extends FilterRequest {
  tag?: string;
}

export interface ReportFilterRequest extends FilterRequest {
  status?: ReportStatus;
}

export interface UserFilterRequest extends FilterRequest {
  role?: string;
}
