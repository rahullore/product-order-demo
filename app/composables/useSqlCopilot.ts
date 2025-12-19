// composables/useSqlCopilot.ts
export type SqlGenerateRequest = { instruction: string };

export type SqlGenerateResponse = {
  sql: string;
  kind: string;
  notes: string;
  tableName: string;
  parameters?: Record<string, any> | null;
};

export type SqlExecuteRequest = {
  tableName: string;
  sql: string;
  parameters?: Record<string, any> | null;
};

export type SqlExecuteResponse = {
  success: boolean;
  kind: string;
  affectedRows: number;
  rows?: Array<Record<string, any>> | null;
};

export type ParamRow = { id: string; name: string; value: string };

export function parseValue(input: string): any {
  const s = input.trim();
  if (s === "") return "";
  if (s.toLowerCase() === "null") return null;
  if (s.toLowerCase() === "true") return true;
  if (s.toLowerCase() === "false") return false;

  if (/^-?\d+(\.\d+)?$/.test(s)) return Number(s);

  const dt = new Date(s);
  if (!Number.isNaN(dt.getTime()) && /^\d{4}-\d{2}-\d{2}/.test(s)) return s;

  return input;
}

export function buildParametersObject(rows: ParamRow[]): Record<string, any> | null {
  const obj: Record<string, any> = {};
  for (const row of rows) {
    const key = row.name.trim();
    if (!key) continue;
    const normalizedKey = key.startsWith("@") ? key.substring(1) : key;
    obj[normalizedKey] = parseValue(row.value);
  }
  return Object.keys(obj).length ? obj : null;
}

export function toParamRows(parameters?: Record<string, any> | null): ParamRow[] {
  const rows: ParamRow[] = [];
  if (!parameters) return rows;

  for (const [k, v] of Object.entries(parameters)) {
    rows.push({
      id: crypto.randomUUID(),
      name: k,
      value: typeof v === "string" ? v : JSON.stringify(v),
    });
  }
  return rows;
}

export function useSqlCopilotApi() {
  const api = useApi();

  const generate = (instruction: string) =>
    api.post<SqlGenerateResponse>("/api/sqlcopilot/generate", { instruction } satisfies SqlGenerateRequest);

  const execute = (req: SqlExecuteRequest) =>
    api.post<SqlExecuteResponse>("/api/sqlcopilot/execute", req);

  return { generate, execute };
}
