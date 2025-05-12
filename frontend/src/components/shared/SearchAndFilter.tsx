
import React, { useState } from "react";
import { Search } from "lucide-react";

interface SearchAndFilterProps {
  onSearch: (searchTerm: string) => void;
  onSortChange?: (sortBy: string, sortDirection: string) => void;
  onTagFilter?: (tag: string) => void;
  tags?: string[];
  sortOptions?: { value: string; label: string }[];
  placeholder?: string;
}

const SearchAndFilter: React.FC<SearchAndFilterProps> = ({
  onSearch,
  onSortChange,
  onTagFilter,
  tags,
  sortOptions = [
    { value: "GameName", label: "По названию игры" },
    { value: "CreatedAt", label: "По дате создания" },
  ],
  placeholder = "Поиск обзоров...",
}) => {
  const [searchTerm, setSearchTerm] = useState("");
  const [sortBy, setSortBy] = useState(sortOptions[0].value);
  const [sortDirection, setSortDirection] = useState("asc");
  const [selectedTag, setSelectedTag] = useState<string | null>(null);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    onSearch(searchTerm);
  };

  const handleSortChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const newSortBy = e.target.value;
    setSortBy(newSortBy);
    if (onSortChange) {
      onSortChange(newSortBy, sortDirection);
    }
  };

  const handleDirectionChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const newDirection = e.target.value;
    setSortDirection(newDirection);
    if (onSortChange) {
      onSortChange(sortBy, newDirection);
    }
  };

  const handleTagClick = (tag: string) => {
    const newTag = selectedTag === tag ? null : tag;
    setSelectedTag(newTag);
    if (onTagFilter) {
      onTagFilter(newTag || "");
    }
  };

  return (
    <div className="space-y-4 mb-6">
      <form onSubmit={handleSearch} className="relative">
        <input
          type="text"
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          placeholder={placeholder}
          className="w-full p-3 pr-10 bg-muted rounded-md focus:outline-none focus:ring-2 focus:ring-primary"
        />
        <button
          type="submit"
          className="absolute right-3 top-1/2 transform -translate-y-1/2 text-muted-foreground hover:text-primary"
        >
          <Search size={20} />
        </button>
      </form>

      <div className="flex flex-wrap items-center gap-4">
        {onSortChange && (
          <div className="flex items-center gap-2">
            <label className="text-sm text-muted-foreground">Сортировать по:</label>
            <select
              value={sortBy}
              onChange={handleSortChange}
              className="bg-muted py-1 px-3 rounded-md focus:outline-none focus:ring-2 focus:ring-primary text-sm"
            >
              {sortOptions.map((option) => (
                <option key={option.value} value={option.value}>
                  {option.label}
                </option>
              ))}
            </select>

            <select
              value={sortDirection}
              onChange={handleDirectionChange}
              className="bg-muted py-1 px-3 rounded-md focus:outline-none focus:ring-2 focus:ring-primary text-sm"
            >
              <option value="asc">По возрастанию</option>
              <option value="desc">По убыванию</option>
            </select>
          </div>
        )}

        {tags && tags.length > 0 && onTagFilter && (
          <div className="flex flex-wrap gap-2">
            {tags.map((tag) => (
              <button
                key={tag}
                onClick={() => handleTagClick(tag)}
                className={`game-tag ${
                  selectedTag === tag ? "bg-primary/40" : ""
                }`}
              >
                {tag}
              </button>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default SearchAndFilter;
