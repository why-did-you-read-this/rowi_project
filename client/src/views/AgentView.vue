<template>
  <CompanyTable title="Компании агентов"
                :showImportant="true"
                :show-priority="true"
                :can-edit="true"
                :show-bank-access="true"
                :fetch-url="'/api/agents/search'"
                :create-url="'/api/agents'"
                :update-url="id => `/api/agents/${id}`"
                :delete-url="id => `/api/agents/${id}`"
                :filters="{
                  inn: true,
                  phone: true,
                  email: true,
                  ogrnDate: true
                }"
                :columns="[
                  { key: 'id', label: 'ID' },
                  { key: 'company', label: 'Компания' },
                  { key: 'contacts', label: 'Контакты' },
                  { key: 'ogrnDateOfAssignment', label: 'Дата присвоения ОГРН' },
                  { key: 'important', label: 'Приоритет' },
                  { key: 'banks', label: 'Банки партнёры' }
                ]"
                @add="openModal"
                @edit="editAgent"
                @delete="deleteAgent" />
  <AgentModal v-if="showModal"
              :agent="editingAgent"
              :isEdit="!!editingAgent"
              @close="closeModal"
              @saved="onModalSaved" />
</template>

<script setup>
  import { ref } from 'vue'
  import CompanyTable from '/src/components/CompanyTable.vue'
  import AgentModal from '/src/components/AgentModal.vue'
  const showModal = ref(false)
  const editingAgent = ref(null)

  function openModal() {
    editingAgent.value = null
    showModal.value = true
  }

  function editAgent(agent) {
    editingAgent.value = agent
    showModal.value = true
  }

  function deleteAgent(agent) {
    if (confirm('Удалить агента?')) {
    }
  }

  function closeModal() {
    showModal.value = false
  }

  function onModalSaved() {
    showModal.value = false
  }

  const columns = [
    { key: 'id', label: 'ID', sortable: true },
    { key: 'repFullName', label: 'ФИО представителя' },
    { key: 'contacts', label: 'Контакты' },
    { key: 'company', label: 'Компания' },
    { key: 'ogrnDateOfAssignment', label: 'Дата присвоения ОГРН', sortable: true },
    { key: 'banks', label: 'Банки' },
    { key: 'important', label: 'Приоритет', sortable: true }
  ]
</script>
