<template>
  <CompanyTable title="Компании клиентов"
                :show-priority="false"
                :show-bank-access="false"
                :fetch-url="'/api/clients/search'"
                :create-url="'/api/clients'"
                :update-url="id => `/api/clients/${id}`"
                :delete-url="id => `/api/clients/${id}`" />
</template>

<script setup>
  import CompanyTable from '/src/components/CompanyTable.vue'
</script>
