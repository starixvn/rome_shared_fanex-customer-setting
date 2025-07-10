import { useState, useRef, useEffect, useCallback, useMemo } from 'react';
import { IconArrowDown } from '@customer-web-app/shared/base/common-icon';
import { useTranslation } from 'react-i18next';
import styles from './single-filterable-dropdown.module.scss';
import classNames from 'classnames';

// Constants
const SEARCH_DEBOUNCE_DELAY = 300;

export interface DropdownOption {
  key: string | number;
  value: string | number;
  label: string | React.ReactNode;
  icon?: React.ReactNode;
  attributes?: Record<string, any>;
}

export interface SingleFilterableDropdownProps {
  dropdownOptions: DropdownOption[];
  onSelect: (option: DropdownOption) => void;
  defaultOption?: DropdownOption;
  className?: string;
  disable?: boolean;
  configs?:{
    searchable?: boolean;
    placeholder?: string;
  };
}

export const SingleFilterableDropdown = ({
  dropdownOptions,
  onSelect,
  defaultOption,
  className,
  disable,
  configs,
}: SingleFilterableDropdownProps) => {
  const { t } = useTranslation();

  // State management
  const [isOpen, setIsOpen] = useState(false);
  const [selected, setSelected] = useState<DropdownOption | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const searchDebounceRef = useRef<NodeJS.Timeout | null>(null);
  const [debouncedSearch, setDebouncedSearch] = useState('');

  // Refs
  const dropdownRef = useRef<HTMLDivElement>(null);
  const searchInputRef = useRef<HTMLInputElement>(null);

  // Initialize selected option when defaultOption changes
  useEffect(() => {
    if (defaultOption && dropdownOptions.length > 0) {
      const option = dropdownOptions.find((opt) => opt.key === defaultOption.key);
      setSelected(option || null);
    }
  }, [defaultOption, dropdownOptions]);

  // Focus search input when dropdown opens
  useEffect(() => {
    if (isOpen && configs?.searchable && searchInputRef.current) {
      searchInputRef.current.focus();
    }
  }, [isOpen, configs?.searchable]);

  // Debounce effect
  useEffect(() => {
    if (searchDebounceRef.current) {
      clearTimeout(searchDebounceRef.current);
    }
    searchDebounceRef.current = setTimeout(() => {
      setDebouncedSearch(searchTerm);
    }, SEARCH_DEBOUNCE_DELAY);
    return () => {
      if (searchDebounceRef.current) {
        clearTimeout(searchDebounceRef.current);
      }
    };
  }, [searchTerm]);

  // Handle click outside to close dropdown
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        closeDropdown();
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  // Helper functions
  const closeDropdown = useCallback(() => {
    setIsOpen(false);
    setSearchTerm('');
  }, []);

  const handleToggle = useCallback(() => {
    if (disable) return;

    if (isOpen) {
      closeDropdown();
    } else {
      setIsOpen(true);
    }
  }, [disable, isOpen, closeDropdown]);

  const handleSelect = useCallback((option: DropdownOption) => {
    if (disable) return;

    setSelected(option);
    closeDropdown();
    onSelect(option);
  }, [disable, closeDropdown, onSelect]);

  const handleSearchChange = useCallback((e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(e.target.value);
  }, []);

  const handleSearchClick = useCallback((e: React.MouseEvent) => {
    e.stopPropagation();
  }, []);

  // Computed values
  const displayOptions = useMemo(() => {
    return dropdownOptions;
  }, [dropdownOptions, t]);

  const filteredOptions = useMemo(() => {
    if (!debouncedSearch) return displayOptions;
    
    return displayOptions.filter((option) => {
      const labelText = typeof option.label === 'string' ? option.label : '';
      return labelText.toLowerCase().includes(debouncedSearch.toLowerCase());
    });
  }, [displayOptions, debouncedSearch]);

  const displayText = useMemo(() => {
    if (selected) {
      const label = selected.label;
      return typeof label === 'string' ? label : '';
    }
    return configs?.placeholder;
  }, [selected, configs?.placeholder, t]);


  const renderSearchInput = () => {
    if (!configs?.searchable) return null;

    return (
      <div className={styles.searchContainer}>
        <input
          ref={searchInputRef}
          type="text"
          value={searchTerm}
          onChange={handleSearchChange}
          placeholder={t('search') || 'Search...'}
          className="control-field"
          onClick={handleSearchClick}
        />
      </div>
    );
  };

  const renderOptions = () => {
    if (filteredOptions.length === 0) {
      return <div className={styles.noData}>{t('noData')}</div>;
    }

    return filteredOptions.map((option, index) => (
      <div
        key={`${option.value}-${index}`}
        onClick={() => handleSelect(option)}
        className={styles.dropdownOption}
      >
        {option.icon}
        <span 
          className={styles.optionLabel} 
        >{option.label}</span>
      </div>
    ));
  };

  return (
    <div className={styles.filterableDropdown} ref={dropdownRef}>
      {/* Control field */}
      <div
        className={`${styles.control} control-field ${disable ? 'disabled' : ''}`}
        onClick={disable ? undefined : handleToggle}
      >
        {selected && selected.icon}
        <span 
          className={styles.selectedText} 
        >{displayText}</span>
        <IconArrowDown className={styles.arrowIcon} />
      </div>

      {/* Dropdown menu */}
      {isOpen && (
        <div className={classNames(styles.dropdownMenu, className)}>
          {renderSearchInput()}
          {renderOptions()}
        </div>
      )}
    </div>
  );
};