
import React, { useState, useEffect } from "react";
import { Search } from "lucide-react";
import Pagination from "../shared/Pagination";
import { reportsAPI } from "../../services/api";
import { Report, ReportStatus, ReportFilterRequest } from "../../types";
import ModerationReportItem from "./ModerationReportItem";

const ModerationReportsList: React.FC = () => {
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

      const response = await reportsAPI.getModerationReports(filter);
      
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
    fetchReports();
  }, [currentPage, sortBy, sortDirection, statusFilter]);

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

  const handleReportStatusUpdated = () => {
    fetchReports();
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

  return (
    <div>
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
          <p className="text-muted-foreground text-lg">Жалоб не найдено</p>
        </div>
      ) : (
        <>
          <div className="space-y-4">
            {reports.map((report) => (
              <ModerationReportItem
                key={report.id}
                report={report}
                onStatusUpdated={handleReportStatusUpdated}
                getStatusName={getStatusName}
              />
            ))}
          </div>

          <Pagination
            currentPage={currentPage}
            totalPages={Math.ceil(totalCount / pageSize)}
            onPageChange={handlePageChange}
          />
        </>
      )}
    </div>
  );
};

export default ModerationReportsList;
