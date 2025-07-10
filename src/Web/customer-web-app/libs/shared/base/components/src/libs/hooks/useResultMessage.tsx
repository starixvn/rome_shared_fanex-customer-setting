import { useState, useCallback, useRef } from "react";
import { ResultMessage } from "../messages/result-message";

export type ResultType = 'success' | 'error' | 'warning' | 'info';

interface ResultState {
    type: ResultType;
    text: string;
}

interface UseResultMessageOptions {
    defaultDuration?: number;
}

interface UseResultMessageReturn {
    currentResult: ResultState | null;
    showResult: (type: ResultType, text: string, duration?: number, onClose?: () => void) => void;
    clearResult: () => void;
    renderResultMessage: () => React.ReactNode;
}

const DEFAULT_DURATION = 3000;

export const useResultMessage = (options: UseResultMessageOptions = {}): UseResultMessageReturn => {
    const { defaultDuration = DEFAULT_DURATION } = options;
    const [result, setResult] = useState<ResultState | null>(null);
    const timeoutRef = useRef<NodeJS.Timeout | null>(null);

    const clearCurrentTimeout = useCallback(() => {
        if (timeoutRef.current) {
            clearTimeout(timeoutRef.current);
            timeoutRef.current = null;
        }
    }, []);

    const showResult = useCallback((
        type: ResultType, 
        text: string, 
        duration: number = defaultDuration, 
        onClose?: () => void
    ) => {
        // Clear any existing timeout
        clearCurrentTimeout();

        // Set the new result
        setResult({ type, text });

        // Set timeout to auto-hide the message
        timeoutRef.current = setTimeout(() => {
            setResult(null);
            onClose?.();
        }, duration);
    }, [defaultDuration, clearCurrentTimeout]);

    const clearResult = useCallback(() => {
        clearCurrentTimeout();
        setResult(null);
    }, [clearCurrentTimeout]);

    const renderResultMessage = useCallback((): React.ReactNode => {
        if (!result) {
            return null;
        }

        return (
            <ResultMessage 
                type={result.type} 
                message={result.text} 
            />
        );
    }, [result]);

    return {
        currentResult: result,
        showResult,
        clearResult,
        renderResultMessage,
    };
};
