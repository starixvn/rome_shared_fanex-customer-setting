import { ChangeEvent, useRef, useEffect, useState, RefObject } from 'react';
import styles from './custom-dropdown.module.scss';
import classNames from 'classnames';

type CustomOption = {
  value: string;
  text: string;
};

function useOutsideAlerter(ref: RefObject<HTMLElement>, clickOutside: () => void): void {
  useEffect(() => {
    function handleClickOutside(event: MouseEvent): void {
      if (ref.current && !ref.current.contains(event.target as Node)) {
        clickOutside();
      }
    }
    // Bind the event listener
    document.addEventListener('mousedown', handleClickOutside);
    return () => {
      // Unbind the event listener on clean up
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, [clickOutside, ref]);
}
const CustomDropdown = ({
  options,
  onChange,
  id,
}: {
  options: CustomOption[];
  onChange: (option: CustomOption) => void;
  id: string;
}) => {
  const [showHideDropdown, setShowHideDropdown] = useState<boolean>(false);
  const [selectedOption, setSelectedOption] = useState<string>('Select');
  const [filteredOptions, setFilteredOptions] = useState<CustomOption[]>(options);
  const wrapperRef = useRef(null);
  useOutsideAlerter(wrapperRef, () => {
    showHideDropdown && setShowHideDropdown(false);
  });

  useEffect(() => {
    setFilteredOptions(options);
  }, [options, showHideDropdown]);

  const handleClickOption = (option: { value: string; text: string; isSelected?: boolean }) => {
    setSelectedOption(option.text);
    onChange(option);
    setShowHideDropdown(false);
  };
  const handleFilerChange = (event: ChangeEvent<HTMLInputElement>) => {
    event.preventDefault();
    const filteredOptions = options.filter((option) => {
      if (option.text.toLowerCase().includes(event.target.value.toLowerCase())) {
        return option;
      }
    });
    setFilteredOptions(filteredOptions);
  };

  return (
    <div ref={wrapperRef} id={id} className={styles['custom-dropdown-container']}>
      <div className={styles['custom-dropdown-result']} onClick={() => setShowHideDropdown(!showHideDropdown)}>
        <span>{selectedOption}</span>
        <span className={styles['arrow']}></span>
      </div>
      {showHideDropdown && (
        <div className={styles['custom-dropdown-search']}>
          <span className={styles['custom-dropdown-search-input']}>
            <input className={styles['custom-dropdown-search-field']} onChange={handleFilerChange} />
          </span>
          <ul className={styles['custom-dropdown-options']} style={{}}>
            {filteredOptions.length === 0 ? (
              <li className={styles['no-result']}>No results found</li>
            ) : (
              filteredOptions.map((option) => (
                <li
                  key={option.value}
                  className={classNames({ [styles.selected]: selectedOption === option.text })}
                  onClick={() => handleClickOption(option)}
                >
                  <span> {option.text}</span>
                </li>
              ))
            )}
          </ul>
        </div>
      )}
    </div>
  );
};

export default CustomDropdown;
