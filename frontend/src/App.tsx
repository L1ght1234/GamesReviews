
import { Toaster } from "@/components/ui/toaster";
import { Toaster as Sonner } from "@/components/ui/sonner";
import { TooltipProvider } from "@/components/ui/tooltip";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import { AuthProvider } from "./context/AuthContext";
import { useEffect } from "react";

// Pages
import HomePage from "./pages/HomePage";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import ProfilePage from "./pages/ProfilePage";
import ReviewDetailPage from "./pages/ReviewDetailPage";
import CreateReviewPage from "./pages/CreateReviewPage";
import EditReviewPage from "./pages/EditReviewPage";
import MyReviewsPage from "./pages/MyReviewsPage";
import MyReportsPage from "./pages/MyReportsPage";
import ModerationDashboard from "./pages/ModerationDashboard";
import NotFoundPage from "./pages/NotFoundPage";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      staleTime: 60000,
      refetchOnWindowFocus: false,
    },
  },
});

const App = () => {
  // Логирование важной информации при инициализации приложения
  useEffect(() => {
    const apiUrl = import.meta.env.VITE_API_URL || "http://localhost:5000";
    console.log(`Приложение инициализировано. API URL: ${apiUrl}`);
  }, []);

  return (
    <QueryClientProvider client={queryClient}>
      <TooltipProvider>
        <AuthProvider>
          <Toaster />
          <Sonner />
          <BrowserRouter>
            <Routes>
              <Route path="/" element={<HomePage />} />
              <Route path="/login" element={<LoginPage />} />
              <Route path="/register" element={<RegisterPage />} />
              <Route path="/profile" element={<ProfilePage />} />
              <Route path="/reviews/:reviewId" element={<ReviewDetailPage />} />
              <Route path="/create-review" element={<CreateReviewPage />} />
              <Route path="/edit-review/:reviewId" element={<EditReviewPage />} />
              <Route path="/my-reviews" element={<MyReviewsPage />} />
              <Route path="/my-reports" element={<MyReportsPage />} />
              <Route path="/moderation" element={<ModerationDashboard />} />
              <Route path="*" element={<NotFoundPage />} />
            </Routes>
          </BrowserRouter>
        </AuthProvider>
      </TooltipProvider>
    </QueryClientProvider>
  );
};

export default App;
