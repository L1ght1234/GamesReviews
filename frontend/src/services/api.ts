import { toast } from "sonner";

// Используем переменную API_URL, которая работает как в разработке, так и в продакшене
const API_URL = import.meta.env.VITE_API_URL || "http://localhost:8080"; 

// Helper function for handling API responses
const handleResponse = async (response: Response) => {
  if (!response.ok) {
    const error = await response.json().catch(() => ({
      message: "Произошла ошибка на сервере",
    }));
    throw new Error(error.message || `HTTP ошибка: ${response.status}`);
  }
  
  if (response.status === 204) {
    return null;
  }
  
  // Check if content type is application/json
  const contentType = response.headers.get("content-type");
  if (contentType && contentType.includes("application/json")) {
    return response.json();
  }
  
  // Handle text responses (like JWT tokens)
  return response.text();
};

// Create request options with authentication token
const createRequestOptions = (
  method: string,
  data?: any,
  contentType = "application/json"
) => {
  const options: RequestInit = {
    method,
    headers: {
      "Content-Type": contentType,
    },
    credentials: "include", // include cookies in requests
  };

  if (data) {
    options.body = JSON.stringify(data);
  }

  return options;
};

// Generic API request function with error handling
export const apiRequest = async (
  endpoint: string,
  method: string,
  data?: any,
  contentType?: string
) => {
  try {
    console.log(`Making ${method} request to ${API_URL}${endpoint}`);
    
    const response = await fetch(
      `${API_URL}${endpoint}`,
      createRequestOptions(method, data, contentType)
    );
    
    console.log(`Response status: ${response.status}`);
    
    return await handleResponse(response);
  } catch (error) {
    console.error("API request error:", error);
    if (error instanceof Error) {
      toast.error(error.message);
    }
    throw error;
  }
};

// Auth API
export const authAPI = {
  login: (data: { email: string; password: string }) =>
    apiRequest("/auth/login", "POST", data),
  register: (data: { userName: string; email: string; password: string }) =>
    apiRequest("/auth/register", "POST", data),
  getProfile: () => apiRequest("/accounts/me", "GET"),
  updateProfile: (data: {
    userName?: string;
    currentPassword?: string;
    newPassword?: string;
    email?: string;
  }) => apiRequest("/accounts/me", "PUT", data),
};

// Reviews API
export const reviewsAPI = {
  getAll: (filter?: any) => {
    const queryParams = filter
      ? `?${new URLSearchParams(
          Object.entries(filter)
            .filter(([_, v]) => v !== null && v !== undefined && v !== "")
            .reduce((acc, [k, v]) => ({ ...acc, [k]: v }), {})
        ).toString()}`
      : "";
    return apiRequest(`/reviews${queryParams}`, "GET");
  },
  getById: (id: string) => apiRequest(`/reviews/${id}`, "GET"),
  getMyReviews: (filter?: any) => {
    const queryParams = filter
      ? `?${new URLSearchParams(
          Object.entries(filter)
            .filter(([_, v]) => v !== null && v !== undefined && v !== "")
            .reduce((acc, [k, v]) => ({ ...acc, [k]: v }), {})
        ).toString()}`
      : "";
    return apiRequest(`/reviews/me${queryParams}`, "GET");
  },
  create: (data: { gameName: string; description: string; tags: string[] }) =>
    apiRequest("/reviews", "POST", data),
  update: (
    id: string,
    data: { gameName: string; description: string; tags: string[] }
  ) => apiRequest(`/reviews/${id}`, "PUT", data),
  delete: (id: string) => apiRequest(`/reviews/${id}`, "DELETE"),
};

// Comments API
export const commentsAPI = {
  getReviewComments: (reviewId: string) =>
    apiRequest(`/reviews/${reviewId}/comments`, "GET"),
  getReplies: (reviewId: string, commentId: string) =>
    apiRequest(`/reviews/${reviewId}/comments/${commentId}/replies`, "GET"),
  createComment: (reviewId: string, data: { text: string }) =>
    apiRequest(`/reviews/${reviewId}/comments`, "POST", data),
  createReply: (reviewId: string, commentId: string, data: { text: string }) =>
    apiRequest(
      `/reviews/${reviewId}/comments/${commentId}/replies`,
      "POST",
      data
    ),
  updateComment: (reviewId: string, commentId: string, data: { text: string }) =>
    apiRequest(`/reviews/${reviewId}/comments/${commentId}`, "PUT", data),
  deleteComment: (reviewId: string, commentId: string) =>
    apiRequest(`/reviews/${reviewId}/comments/${commentId}`, "DELETE"),
};

// Reports API
export const reportsAPI = {
  getMyReports: (filter?: any) => {
    const queryParams = filter
      ? `?${new URLSearchParams(
          Object.entries(filter)
            .filter(([_, v]) => v !== null && v !== undefined && v !== "")
            .reduce((acc, [k, v]) => ({ ...acc, [k]: v }), {})
        ).toString()}`
      : "";
    return apiRequest(`/api/reports/me${queryParams}`, "GET");
  },
  getModerationReports: (filter?: any) => {
    const queryParams = filter
      ? `?${new URLSearchParams(
          Object.entries(filter)
            .filter(([_, v]) => v !== null && v !== undefined && v !== "")
            .reduce((acc, [k, v]) => ({ ...acc, [k]: v }), {})
        ).toString()}`
      : "";
    return apiRequest(`/api/reports/moderation${queryParams}`, "GET");
  },
  create: (data: {
    reportedUserId: string;
    contentId: string;
    contentType: number;
    reason: string;
    description: string;
  }) => apiRequest("/api/reports", "POST", data),
  updateStatus: (reportId: string, data: { status: number }) =>
    apiRequest(`/api/reports/moderation/${reportId}`, "PUT", data),
  getReportedContent: (contentType: number, contentId: string) =>
    apiRequest(`/api/reports/content/${contentType}/${contentId}`, "GET"),
};

// Moderation API
export const moderationAPI = {
  getUsers: (filter?: any) => {
    const queryParams = filter
      ? `?${new URLSearchParams(
          Object.entries(filter)
            .filter(([_, v]) => v !== null && v !== undefined && v !== "")
            .reduce((acc, [k, v]) => ({ ...acc, [k]: v }), {})
        ).toString()}`
      : "";
    return apiRequest(`/accounts/moderation${queryParams}`, "GET");
  },
  updateUser: (
    userId: string,
    data: { userName?: string; newPassword?: string; email?: string }
  ) => apiRequest(`/accounts/moderation/${userId}`, "PUT", data),
  deleteUser: (userId: string) =>
    apiRequest(`/accounts/moderation/${userId}`, "DELETE"),
};

// Admin API
export const adminAPI = {
  createModerator: (data: {
    userName: string;
    email: string;
    password: string;
  }) => apiRequest("/accounts/administration/moderators", "POST", data),
};
