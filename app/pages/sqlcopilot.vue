<template>
  <div class="min-h-screen bg-gray-950 text-gray-100">
    <div class="mx-auto max-w-6xl p-6 space-y-6">

      <!-- Page header -->
      <header>
        <h1 class="text-2xl font-semibold">SQL Copilot</h1>
        <p class="text-gray-400">
          Generate SQL from natural language, review it, and execute safely.
        </p>
      </header>

      <!-- Instruction + Editor -->
      <div class="grid gap-6 lg:grid-cols-2">
        <SqlcopilotInstructionPanel
          v-model="instruction"
          :is-loading="isGenerating"
          :error="generateError"
          :summary="generatedSummary"
          @generate="onGenerate"
          @reset="resetAll"
        />

        <SqlcopilotEditorPanel
          v-model:tableName="tableName"
          v-model:sql="sql"
          v-model:parameters="paramRows"
          :is-loading="isExecuting"
          :error="executeError"
          @execute="onExecute"
        />
      </div>

      <!-- Result -->
      <SqlcopilotResultPanel :result="executed" />

    </div>
  </div>
</template>

<script setup lang="ts">
import {
  useSqlCopilotApi,
  toParamRows,
  buildParametersObject,
  type ParamRow,
  type SqlGenerateResponse,
  type SqlExecuteResponse
} from "~/composables/useSqlCopilot";

/* -------------------- API -------------------- */
const { generate, execute } = useSqlCopilotApi();

/* -------------------- STATE -------------------- */
const instruction = ref("");

const isGenerating = ref(false);
const generateError = ref<string | null>(null);
const generated = ref<SqlGenerateResponse | null>(null);

const tableName = ref("");
const sql = ref("");
const paramRows = ref<ParamRow[]>([]);

const isExecuting = ref(false);
const executeError = ref<string | null>(null);
const executed = ref<SqlExecuteResponse | null>(null);

/* -------------------- DERIVED -------------------- */
const generatedSummary = computed(() =>
  generated.value
    ? {
        kind: generated.value.kind,
        tableName: generated.value.tableName,
        notes: generated.value.notes
      }
    : null
);

/* -------------------- ACTIONS -------------------- */
async function onGenerate() {
  isGenerating.value = true;
  generateError.value = null;
  executed.value = null;

  try {
    const resp = await generate(instruction.value);
    generated.value = resp;

    // hydrate editor
    tableName.value = resp.tableName ?? "";
    sql.value = resp.sql ?? "";
    paramRows.value = toParamRows(resp.parameters);
  } catch (e: any) {
    generateError.value =
      e?.data?.message ?? e?.message ?? "SQL generation failed";
  } finally {
    isGenerating.value = false;
  }
}

async function onExecute() {
  isExecuting.value = true;
  executeError.value = null;

  try {
    const resp = await execute({
      tableName: tableName.value,
      sql: sql.value,
      parameters: buildParametersObject(paramRows.value)
    });

    executed.value = resp;
  } catch (e: any) {
    executeError.value =
      e?.data?.message ?? e?.message ?? "SQL execution failed";
  } finally {
    isExecuting.value = false;
  }
}

function resetAll() {
  instruction.value = "";
  generated.value = null;
  tableName.value = "";
  sql.value = "";
  paramRows.value = [];
  executed.value = null;
  generateError.value = null;
  executeError.value = null;
}
</script>
