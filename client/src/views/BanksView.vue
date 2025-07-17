<template>
  <CompanyTable title="Банки партнёры"
                :show-priority="false"
                :show-bank-access="false"
                :fetch-url="'/api/banks/search'"
                :create-url="'/api/banks'"
                :update-url="id => `/api/banks/${id}`"
                :delete-url="id => `/api/banks/${id}`" />
</template>

<script setup>
  import CompanyTable from '/src/components/CompanyTable.vue'
</script>
