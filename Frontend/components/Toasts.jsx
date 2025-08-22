import React, { useCallback, useContext, useEffect, useMemo, useRef, useState } from 'react';

// ToastContext provides { open, close }
export const ToastContext = React.createContext(null);

let globalToastCounter = 0;

// A single Toast element (no animations per request)
export function Toast({ id, message, duration, color, onClose }) {
    // shadcn-inspired base (border, background, shadow). If no explicit color prop, fall back to bg-identity.
    const baseClasses =
        "pointer-events-auto select-none rounded-md border shadow-md ring-1 ring-black/5 px-4 py-3 text-sm flex items-start gap-3 w-full";
    const themedClasses = color
        ? " text-white" // custom colored background; force white text
        : " bg-identity text-background"; // fallback theme class requested
    return (
        <div
            className={baseClasses + themedClasses}
            style={color ? { backgroundColor: color } : undefined}
            role="status"
            aria-live="polite"
        >
            <div className="flex-1 whitespace-pre-wrap break-words">{message}</div>
            <button
                type="button"
                onClick={() => onClose(id)}
                className="opacity-70 hover:opacity-100 transition focus:outline-none"
                aria-label="Close toast"
            >
                ×
            </button>
        </div>
    );
}

/**
 * ToastProvider wraps your app. It renders children normally plus a fixed overlay that doesn't block interaction.
 * open(message: string, duration?: number, color?: string)
 * close(id: number)
 */
export function ToastProvider({ children, maxToasts = 5, defaultDuration = 3000 }) {
    const [toasts, setToasts] = useState([]); // { id, message, duration, color, createdAt }
    const timeoutsRef = useRef(new Map());

    const close = useCallback((id) => {
        setToasts((prev) => prev.filter((t) => t.id !== id));
        const handle = timeoutsRef.current.get(id);
        if (handle) {
            clearTimeout(handle);
            timeoutsRef.current.delete(id);
        }
    }, []);

    const open = useCallback((message, duration, color) => {
        const id = ++globalToastCounter;
        const toast = {
            id,
            message: String(message ?? ''),
            duration: typeof duration === 'number' ? duration : defaultDuration,
            color: color || null,
            createdAt: Date.now(),
        };
        setToasts((prev) => {
            const next = [...prev, toast];
            // enforce cap
            if (next.length > maxToasts) next.splice(0, next.length - maxToasts);
            return next;
        });
        // schedule auto close
        const handle = setTimeout(() => {
            close(id);
        }, toast.duration);
        timeoutsRef.current.set(id, handle);
        return id;
    }, [close, defaultDuration, maxToasts]);

    // Clean up on unmount
    useEffect(() => () => {
        timeoutsRef.current.forEach((h) => clearTimeout(h));
        timeoutsRef.current.clear();
    }, []);

    const value = useMemo(() => ({ open, close }), [open, close]);

    return (
        <ToastContext.Provider value={value}>
            {children}
            {/* Overlay container. pointer-events-none so underlying UI is clickable. Individual toasts re-enable pointer events. */}
            <div className="pointer-events-none fixed inset-0 z-[9999] flex flex-col items-center gap-2 p-4">
                {/* Top-center stack */}
                <div className="flex flex-col items-center gap-2 w-full max-w-sm" style={{ pointerEvents: 'none' }}>
                    {toasts.map((t) => (
                        <div key={t.id} style={{ pointerEvents: 'auto' }} className="w-full">
                            <Toast id={t.id} message={t.message} duration={t.duration} color={t.color || undefined} onClose={close} />
                        </div>
                    ))}
                </div>
            </div>
        </ToastContext.Provider>
    );
}

// Hook to access toast context
export function useToast() {
    const ctx = useContext(ToastContext);
    if (!ctx) {
        // Fail fast so devs know they forgot the provider
        throw new Error('useToast must be used within <ToastProvider>');
    }
    return ctx;
}

// Optional default export grouping
export default { ToastProvider, ToastContext, useToast, Toast };
