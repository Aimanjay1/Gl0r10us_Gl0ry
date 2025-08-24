export function toUtcIsoWithCurrentTime(dateOnly) {
    if (!dateOnly) return null;
    const now = new Date();
    const [y, m, d] = dateOnly.split("-").map(Number);
    return new Date(Date.UTC(
        y, m - 1, d,
        now.getHours(), now.getMinutes(), now.getSeconds(), now.getMilliseconds()
    )).toISOString();
}
export function formatDate(isoString) {
    if (!isoString) return "";
    return new Date(isoString).toLocaleString("en-MY", {
        year: "numeric",
        month: "short",
        day: "2-digit",
        hour: "2-digit",
        minute: "2-digit",
        second: "2-digit",
        hour12: false,
        timeZone: "Asia/Kuala_Lumpur"
    });
}

export function formatDateOnly(isoString) {
    if (!isoString) return "";
    return new Date(isoString).toLocaleDateString("en-MY", {
        year: "numeric",
        month: "short",
        day: "2-digit",
        timeZone: "Asia/Kuala_Lumpur"
    });
}

// Usage:
const readable = formatDate("2025-08-23T09:59:12.819Z"); // "23 Aug 2025, 17:59:12"