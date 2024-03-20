import { createContext } from 'react';

import { ValidationResult } from '@customer-web-app/share-models';

interface ValidationContextType {
  validationState: ValidationResult;
  setValidationState: (validationResult: ValidationResult) => void;
}

export const ValidationContext = createContext<Partial<ValidationContextType>>({});
