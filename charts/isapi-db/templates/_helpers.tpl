{{/* vim: set filetype=mustache: */}}
{{/*
Expand the name of the chart.
*/}}
{{- define "isapi-db.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "isapi-db.fullname" -}}
{{- if .Values.fullnameOverride -}}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- $name := default .Chart.Name .Values.nameOverride -}}
{{- if contains $name .Release.Name -}}
{{- .Release.Name | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" -}}
{{- end -}}
{{- end -}}
{{- end -}}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "isapi-db.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/*
Common labels
*/}}
{{- define "isapi-db.labels" -}}
helm.sh/chart: {{ include "isapi-db.chart" . }}
{{ include "isapi-db.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end -}}

{{/*
Selector labels
*/}}
{{- define "isapi-db.selectorLabels" -}}
app.kubernetes.io/name: {{ include "isapi-db.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end -}}

{{/*
Create the name of the service account to use
*/}}
{{- define "isapi-db.serviceAccountName" -}}
{{- if .Values.serviceAccount.create -}}
    {{ default (include "isapi-db.fullname" .) .Values.serviceAccount.name }}
{{- else -}}
    {{ default "default" .Values.serviceAccount.name }}
{{- end -}}
{{- end -}}

{{/*
Defines which image:tag and what pull policy to use
*/}}
{{- define "isapi-db.image.properties" -}}
{{- $localImageName := .Values.image.repository | replace "gpitfuturesdevacr.azurecr.io/" "" -}}
{{- $imageName := ternary $localImageName (printf "%s:%s" .Values.image.repository .Chart.AppVersion) .Values.useLocalImage -}}
{{- $imagePullPolicy := ternary "IfNotPresent" "Always" .Values.useLocalImage -}}
image: {{ $imageName | quote }}
imagePullPolicy: {{ $imagePullPolicy | quote }}
{{- end }}