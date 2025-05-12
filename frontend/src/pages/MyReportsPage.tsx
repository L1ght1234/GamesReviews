
import React, { useState, useEffect } from "react";
import Layout from "../components/layout/Layout";
import Pagination from "../components/shared/Pagination";
import { reportsAPI } from "../services/api";
import { Report, ReportStatus, ReportFilterRequest } from "../types";
import { useAuth } from "../context/AuthContext";
import { Link } from "react-router-dom";
import { Search } from "lucide-react";

const MyReportsPage: React.FC = () => {
  const { isAuthenticated } = useAuth();
  const [reports, setReports] = useState<Report[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [totalCount, setTotalCount] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize] = useState(10);
  const [searchTerm, setSearchTerm] = useState("");
  const [statusFilter, setStatusFilter] = useState<ReportStatus | "">("");
  const [sortBy, setSortBy] = useState("CreatedAt");
  const [sortDirection, setSortDirection] = useState("desc");

  const fetchReports = async () => {
    try {
      setLoading(true);
      const filter: ReportFilterRequest = {
        search: searchTerm,
        status: statusFilter !== "" ? (statusFilter as ReportStatus) : undefined,
        sortBy,
        sortDirection,
        page: currentPage,
        pageSize,
      };

      const response = await reportsAPI.getMyReports(filter);
      
      setReports(response.items);
      setTotalCount(response.totalCount);
      setError("");
    } catch (err) {
      console.error("Error fetching reports:", err);
      setError("Не удалось загрузить список жалоб");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (isAuthenticated) {
      fetchReports();
    } else {
      setLoading(false);
    }
  }, [currentPage, sortBy, sortDirection, statusFilter, isAuthenticated]);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    setCurrentPage(1);
    fetchReports();
  };

  const handleStatusChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const value = e.target.value;
    setStatusFilter(value === "" ? "" : parseInt(value) as ReportStatus);
    setCurrentPage(1);
  };

  const handleSortChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setSortBy(e.target.value);
    setCurrentPage(1);
  };

  const handleDirectionChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setSortDirection(e.target.value);
    setCurrentPage(1);
  };

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
  };

  const getStatusName = (status: ReportStatus) => {
    switch (status) {
      case ReportStatus.InProgress:
        return "В рассмотрении";
      case ReportStatus.Resolved:
        return "Решено";
      case ReportStatus.Dismissed:
        return "Отклонено";
      default:
        return "Неизвестно";
    }
  };

  const getStatusBadgeClass = (status: ReportStatus) => {
    switch (status) {
      case ReportStatus.InProgress:
        return "bg-primary/20 text-primary";
      case ReportStatus.Resolved:
        return "bg-green-500/20 text-green-500";
      case ReportStatus.Dismissed:
        return "bg-orange-500/20 text-orange-500";
      default:
        return "bg-muted text-muted-foreground";
    }
  };

  if (!isAuthenticated) {
    return (
      <Layout>
        <div className="max-w-2xl mx-auto">
          <div className="bg-card border border-border rounded-lg p-6">
            <h1 className="text-xl font-semibold mb-4">Требуется авторизация</h1>
            <p>Для просмотра ваших жалоб необходимо войти в аккаунт.</p>
          </div>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="max-w-5xl mx-auto">
        <h1 className="text-3xl font-bold mb-6">Мои жалобы</h1>

        <div className="mb-6">
          <form onSubmit={handleSearch} className="flex flex-wrap gap-4">
            <div className="relative flex-grow">
              <input
                type="text"
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                placeholder="Поиск по жалобам..."
                className="w-full p-3 pr-10 bg-muted rounded-md focus:outline-none focus:ring-2 focus:ring-primary"
              />
              <button
                type="submit"
                className="absolute right-3 top-1/2 transform -translate-y-1/2 text-muted-foreground hover:text-primary"
              >
                <Search size={20} />
              </button>
            </div>
            
            <div className="flex gap-2">
              <select
                value={statusFilter === "" ? "" : statusFilter.toString()}
                onChange={handleStatusChange}
                className="bg-muted p-3 rounded-md focus:outline-none focus:ring-2 focus:ring-primary"
              >
                <option value="">Все статусы</option>
                <option value={ReportStatus.InProgress.toString()}>
                  В рассмотрении
                </option>
                <option value={ReportStatus.Resolved.toString()}>Решено</option>
                <option value={ReportStatus.Dismissed.toString()}>Отклонено</option>
              </select>
              
              <select
                value={sortBy}
                onChange={handleSortChange}
                className="bg-muted p-3 rounded-md focus:outline-none focus:ring-2 focus:ring-primary"
              >
                <option value="CreatedAt">По дате</option>
                <option value="Status">По статусу</option>
                <option value="Reason">По причине</option>
              </select>
              
              <select
                value={sortDirection}
                onChange={handleDirectionChange}
                className="bg-muted p-3 rounded-md focus:outline-none focus:ring-2 focus:ring-primary"
              >
                <option value="asc">По возрастанию</option>
                <option value="desc">По убыванию</option>
              </select>
            </div>
          </form>
        </div>

        {loading ? (
          <div className="flex justify-center py-12">
            <div className="animate-spin h-12 w-12 border-4 border-primary border-t-transparent rounded-full"></div>
          </div>
        ) : error ? (
          <div className="bg-destructive/10 text-destructive p-4 rounded-md">
            {error}
          </div>
        ) : reports.length === 0 ? (
          <div className="text-center py-12">
            <p className="text-muted-foreground text-lg">У вас нет жалоб</p>
          </div>
        ) : (
          <>
            <div className="space-y-4">
              {reports.map((report) => {
                const formattedDate = new Date(report.createdAt).toLocaleDateString(
                  "ru-RU",
                  {
                    day: "numeric",
                    month: "long",
                    year: "numeric",
                    hour: "2-digit",
                    minute: "2-digit",
                  }
                );

                return (
                  <div key={report.id} className="bg-card border border-border rounded-lg p-4">
                    <div className="flex justify-between items-start mb-3">
                      <div>
                        <span
                          className={`px-2 py-1 rounded text-xs ${getStatusBadgeClass(
                            report.status
                          )}`}
                        >
                          {getStatusName(report.status)}
                        </span>
                        <span className="ml-2 text-xs text-muted-foreground">
                          {formattedDate}
                        </span>
                      </div>
                    </div>

                    <div className="mb-3">
                      <h3 className="font-medium">Причина:</h3>
                      <p className="text-sm">{report.reason}</p>
                    </div>

                    {report.description && (
                      <div className="mb-3">
                        <h3 className="font-medium">Описание:</h3>
                        <p className="text-sm whitespace-pre-wrap">
                          {report.description}
                        </p>
                      </div>
                    )}

                    <div className="mb-3">
                      <h3 className="font-medium">Тип контента:</h3>
                      <p className="text-sm">
                        {report.contentType === 0 ? "Обзор" : "Комментарий"}
                      </p>
                    </div>
                  </div>
                );
              })}
            </div>

            <Pagination
              currentPage={currentPage}
              totalPages={Math.ceil(totalCount / pageSize)}
              onPageChange={handlePageChange}
            />
          </>
        )}
      </div>
    </Layout>
  );
};

export default MyReportsPage;
