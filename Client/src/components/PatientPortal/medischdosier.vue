<template>
    
	<div class="space-y-4">
		<header class="space-y-1">
			<h2 class="text-xl font-semibold">Medisch dossier</h2>
			<p class="text-sm text-gray-600">
				Overzicht van uw medische geschiedenis en documenten.
			</p>
		</header>

		<div class="space-y-3">
			<div class="flex items-center justify-between gap-2">
				<h3 class="text-lg font-semibold">Medische geschiedenis</h3>
				<button
					class="rounded-xl border px-3 py-2 text-sm"
					type="button"
					@click="loadDosier"
					:disabled="loading"
				>
					{{ loading ? "Laden..." : "Verversen" }}
				</button>
			</div>

			<div v-if="error" class="rounded-lg border border-red-200 bg-red-50 p-3 text-sm text-red-700">
				{{ error }}
			</div>

			<ul v-if="sortedDosier.length" class="space-y-3">
				<li
					v-for="item in sortedDosier"
					:key="item.lineNumber"
					class="rounded-xl border border-slate-200 bg-white p-4 shadow-sm"
				>
					<div class="space-y-1">
						<div class="text-base font-semibold text-slate-900">
							{{ item.patientName ?? "Patiënt" }}
						</div>
						<div class="text-sm text-slate-600">
							{{ item.description || "Geen aanvullende notities" }}
						</div>
					</div>

					<div class="mt-3 grid gap-1 text-xs text-slate-500 sm:grid-cols-2">
						<div>
							<span class="font-medium text-slate-600">Patiëntnummer:</span>
							{{ item.patientNumber ?? "-" }}
						</div>
						<div>
							<span class="font-medium text-slate-600">Regelnummer:</span>
							{{ item.lineNumber ?? "-" }}
						</div>
						<div>
							<span class="font-medium text-slate-600">Datum:</span>
							{{ formatDate(item.createdOn) }}
						</div>
						<div>
							<span class="font-medium text-slate-600">Bijlage:</span>
							{{ item.file ? "Aanwezig" : "Geen" }}
						</div>
					</div>

					<div v-if="item.file" class="mt-4 flex gap-2">
						<button
							class="rounded-xl border border-blue-600 px-3 py-2 text-sm font-medium text-blue-700 shadow-sm hover:bg-blue-50"
							type="button"
							@click="viewFile(item)"
						>
							Bekijk PDF
						</button>
						<button
							class="rounded-xl bg-blue-600 px-3 py-2 text-sm font-medium text-white shadow-sm hover:bg-blue-700"
							type="button"
							@click="downloadFile(item)"
						>
							Download bestand
						</button>
					</div>
				</li>
			</ul>

			<div v-else class="rounded-xl border border-dashed border-slate-200 p-6 text-center">
				<p class="text-sm text-gray-600">Geen medische gegevens beschikbaar.</p>
			</div>
		</div>
	</div>
</template>

<script>
import { getPatientMedicalRecords } from "../../services/medicalRecordsService";

export default {
	name: "MedischDosier",
	props: {
		patientNumber: { type: [String, Number], default: "" }
	},
	data() {
		return {
			dosier: [],
			loading: false,
			error: ""
		};
	},
	computed: {
		sortedDosier() {
			return [...this.dosier].sort(
				(a, b) => new Date(b.createdOn) - new Date(a.createdOn)
			);
		}
	},
	methods: {
		async loadDosier() {
			this.loading = true;
			this.error = "";

			try {
				const data = await getPatientMedicalRecords(this.patientNumber || null);
				this.dosier = Array.isArray(data) ? data : [];
			} catch (e) {
				this.error =
					e?.response?.data ?? e?.message ?? "Kon medisch dossier niet ophalen.";
				this.dosier = [];
			} finally {
				this.loading = false;
			}
		},
		formatDate(value) {
			if (!value) return "-";
			const d = new Date(value);
			return isNaN(d.getTime()) ? "-" : d.toLocaleString("nl-NL");
		},
		buildPdfBlob(item) {
			if (!item?.file) return null;
			const data = item.file;

			let base64 = data;
			let mime = "application/pdf";

			if (typeof data === "string" && data.startsWith("data:")) {
				const [meta, payload] = data.split(",");
				base64 = payload ?? "";
				const match = meta.match(/data:([^;]+);base64/);
				if (match?.[1]) mime = match[1];
			}

			const byteCharacters = atob(base64);
			const byteNumbers = new Array(byteCharacters.length);
			for (let i = 0; i < byteCharacters.length; i++) {
				byteNumbers[i] = byteCharacters.charCodeAt(i);
			}
			const byteArray = new Uint8Array(byteNumbers);
			const blob = new Blob([byteArray], { type: mime || "application/pdf" });
			return { blob, mime: mime || "application/pdf" };
		},
		viewFile(item) {
			try {
				const result = this.buildPdfBlob(item);
				if (!result) return;
				const url = URL.createObjectURL(result.blob);
				window.open(url, "_blank", "noopener,noreferrer");
				setTimeout(() => URL.revokeObjectURL(url), 1000);
			} catch {
				this.error = "PDF kon niet worden geopend.";
			}
		},
		downloadFile(item) {
			try {
				const result = this.buildPdfBlob(item);
				if (!result) return;
				const { blob } = result;
				const url = URL.createObjectURL(blob);
				const link = document.createElement("a");
				link.href = url;
				link.download = `medical-record-${item.lineNumber ?? "file"}.pdf`;
				document.body.appendChild(link);
				link.click();
				document.body.removeChild(link);
				URL.revokeObjectURL(url);
			} catch {
				this.error = "Bijlage kon niet worden gedownload.";
			}
		}
	},
	mounted() {
		this.loadDosier();
	},
	watch: {
		patientNumber() {
			this.loadDosier();
		}
	}
};
</script>
