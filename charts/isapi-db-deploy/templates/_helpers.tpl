{{/* vim: set filetype=mustache: */}}
{{/*
Expand the name of the chart.
*/}}
{{- define "isapi-db-deploy.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "isapi-db-deploy.fullname" -}}
{{- $name := include "isapi-db-deploy.chart" . -}}

{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-"  -}}
{{- end -}}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "isapi-db-deploy.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | trunc 63 | replace "+" "-" | replace "." "-" | lower | trimSuffix "-" -}}
{{- end -}}

{{/*
Common labels
*/}}
{{- define "isapi-db-deploy.labels" -}}
helm.sh/chart: {{ include "isapi-db-deploy.chart" . }}
{{ include "isapi-db-deploy.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end -}}

{{/*
Selector labels
*/}}
{{- define "isapi-db-deploy.selectorLabels" -}}
app.kubernetes.io/name: {{ include "isapi-db-deploy.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end -}}

{{/*
Create the name of the service account to use
*/}}
{{- define "isapi-db-deploy.serviceAccountName" -}}
{{- if .Values.serviceAccount.create -}}
    {{ default (include "isapi-db-deploy.fullname" .) .Values.serviceAccount.name }}
{{- else -}}
    {{ default "default" .Values.serviceAccount.name }}
{{- end -}}
{{- end -}}

{{/*
Defines which image:tag and what pull policy to use
*/}}
{{- define "isapi-db-deploy.image.properties" -}}
{{- $localImageName := .Values.image.repository | replace "gpitfuturesdevacr.azurecr.io/" "" -}}
{{- $imageName := ternary $localImageName (printf "%s:%s" .Values.image.repository .Chart.AppVersion) .Values.useLocalImage -}}
image: {{ $imageName | quote }}
imagePullPolicy: {{ "IfNotPresent" | quote }}
{{- end }}
