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
