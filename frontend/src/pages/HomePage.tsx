
import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import Layout from "../components/layout/Layout";
import ReviewCard from "../components/reviews/ReviewCard";
import SearchAndFilter from "../components/shared/SearchAndFilter";
import Pagination from "../components/shared/Pagination";
import { reviewsAPI } from "../services/api";
import { Review, ReviewFilterRequest } from "../types";
import { useAuth } from "../context/AuthContext";

const HomePage: React.FC = () => {
  const { isAuthenticated } = useAuth();
  const [reviews, setReviews] = useState<Review[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [tags, setTags] = useState<string[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize] = useState(10);
  
  // Filter state
  const [filter, setFilter] = useState<ReviewFilterRequest>({
    search: "",
    tag: "",
    sortBy: "GameName",
    sortDirection: "asc",
    page: currentPage,
    pageSize: pageSize,
  });

  const fetchReviews = async () => {
    try {
      setLoading(true);
      const response = await reviewsAPI.getAll({
        ...filter,
        page: currentPage,
      });
      
      setReviews(response.items);
      setTotalCount(response.totalCount);
      
      // Extract unique tags - исправляем типизацию
      const allTags = response.items.flatMap((review) => review.tags);
      const uniqueTags = [...new Set(allTags)].filter((tag): tag is string => 
        typeof tag === 'string'
      );
      setTags(uniqueTags);
      
      setError("");
    } catch (err) {
      console.error("Error fetching reviews:", err);
      setError("Не удалось загрузить обзоры");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchReviews();
  }, [currentPage, filter.sortBy, filter.sortDirection, filter.tag, filter.search]);

  const handleSearch = (searchTerm: string) => {
    setFilter({ ...filter, search: searchTerm });
    setCurrentPage(1);
  };

  const handleSortChange = (sortBy: string, sortDirection: string) => {
    setFilter({ ...filter, sortBy, sortDirection });
    setCurrentPage(1);
  };

  const handleTagFilter = (tag: string) => {
    setFilter({ ...filter, tag });
    setCurrentPage(1);
  };

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
  };

  return (
    <Layout>
      <div className="max-w-5xl mx-auto">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-3xl font-bold">Обзоры игр</h1>
          
          {isAuthenticated && (
            <Link to="/create-review" className="btn-primary">
              Написать обзор
            </Link>
          )}
        </div>

        <SearchAndFilter
          onSearch={handleSearch}
          onSortChange={handleSortChange}
          onTagFilter={handleTagFilter}
          tags={tags}
        />

        {loading ? (
          <div className="flex justify-center py-12">
            <div className="animate-spin h-12 w-12 border-4 border-primary border-t-transparent rounded-full"></div>
          </div>
        ) : error ? (
          <div className="bg-destructive/10 text-destructive p-4 rounded-md">
            {error}
          </div>
        ) : reviews.length === 0 ? (
          <div className="text-center py-12">
            <p className="text-muted-foreground text-lg">Не найдено ни одного обзора</p>
            {isAuthenticated && (
              <Link to="/create-review" className="btn-primary mt-4 inline-block">
                Создать первый обзор
              </Link>
            )}
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {reviews.map((review) => (
              <ReviewCard key={review.reviewId} review={review} />
            ))}
          </div>
        )}

        <Pagination
          currentPage={currentPage}
          totalPages={Math.ceil(totalCount / pageSize)}
          onPageChange={handlePageChange}
        />
      </div>
    </Layout>
  );
};

export default HomePage;
