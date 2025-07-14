<template>
  <div class="app-container min-h-screen bg-gray-50 text-gray-800">
    <!-- Навигация -->
    <nav class="nav-tabs bg-white shadow mb-6">
      <RouterLink v-for="tab in tabs"
                  :key="tab.route"
                  :to="tab.route"
                  class="tab-item px-4 py-3 inline-block text-gray-600 hover:text-blue-600"
                  active-class="text-blue-600 border-b-2 border-blue-600">
        {{ tab.name }}
      </RouterLink>
    </nav>

    <!-- Фильтры -->
    <section class="filter-section bg-white p-6 rounded-xl shadow mb-6">
      <h3 class="text-lg font-semibold mb-4">Настройка фильтра</h3>
      <div class="filter-grid grid grid-cols-1 md:grid-cols-3 lg:grid-cols-4 gap-4">
        <div>
          <label class="block mb-1 text-sm font-medium">ИНН</label>
          <input v-model="filters.inn" type="text" placeholder="Введите ИНН" class="border rounded-xl px-3 py-2 w-full" />
        </div>
        <div>
          <label class="block mb-1 text-sm font-medium">Телефон</label>
          <input v-model="filters.repPhoneNumber" type="text" placeholder="Введите телефон" class="border rounded-xl px-3 py-2 w-full" />
        </div>
        <div>
          <label class="block mb-1 text-sm font-medium">Email</label>
          <input v-model="filters.repEmail" type="email" placeholder="Введите email" class="border rounded-xl px-3 py-2 w-full" />
        </div>
        <div class="flex items-center mt-6">
          <input :checked="filters.important === true"
                 @change="filters.important = $event.target.checked ? true : null"
                 type="checkbox" id="priority-filter" class="mr-2" />
          <label for="priority-filter" class="text-sm">Только приоритетные</label>
        </div>
        <div>
          <label class="block mb-1 text-sm font-medium">ОГРН от</label>
          <input v-model="filters.ogrnDateFrom" type="date" class="border rounded-xl px-3 py-2 w-full" />
        </div>
        <div>
          <label class="block mb-1 text-sm font-medium">ОГРН до</label>
          <input v-model="filters.ogrnDateTo" type="date" class="border rounded-xl px-3 py-2 w-full" />
        </div>
      </div>
      <button @click="applyFilter" class="bg-blue-600 text-white px-4 py-2 rounded-xl hover:bg-blue-700 mt-4">
        Применить фильтрацию
      </button>
    </section>

    <!-- Сортировка -->
    <!--<section class="sort-controls mb-4 flex gap-4 items-center">
      <label class="text-sm font-medium">Сортировать по:</label>
      <select v-model="filters.sortBy" @change="applyFilter" class="input w-auto">
        <option value="id">ID</option>
        <option value="ogrnDateOfAssignment">Дата ОГРН</option>
        <option value="important">Приоритет</option>
      </select>

      <select v-model="filters.sortDirection" @change="applyFilter" class="input w-auto">
        <option value="asc">↑ По возрастанию</option>
        <option value="desc">↓ По убыванию</option>
      </select>
    </section>-->

    <!-- Таблица -->
    <section class="bg-white rounded-xl shadow p-6 mb-6 relative">
      <button @click="openModal" class="absolute top-0 right-0 bg-green-600 text-white px-4 py-2 rounded-xl hover:bg-green-700">
        + Добавить
      </button>

      <table class="min-w-full text-sm">
        <thead class="bg-gray-100">
          <tr>
            <th class="th text-left px-3 py-2 cursor-pointer"
                @click="setSort('id')">
              ID
              <span v-if="filters.sortBy === 'id'">
                {{ filters.sortDirection === 'asc' ? '▲' : '▼' }}
              </span>
            </th>
            <th class="th text-left px-3 py-2">ФИО представителя</th>
            <th class="th text-left px-3 py-2">Контакты</th>
            <th class="th text-left px-3 py-2">Компания</th>
            <th class="th text-left px-3 py-2 cursor-pointer"
                @click="setSort('ogrnDateOfAssignment')">
              Дата присвоения ОГРН
              <span v-if="filters.sortBy === 'ogrnDateOfAssignment'">
                {{ filters.sortDirection === 'asc' ? '▲' : '▼' }}
              </span>
            </th>
            <th class="th text-left px-3 py-2">Банки</th>
            <th class="th text-left px-3 py-2 cursor-pointer"
                @click="setSort('important')">
              Приоритет
              <span v-if="filters.sortBy === 'important'">
                {{ filters.sortDirection === 'asc' ? '▲' : '▼' }}
              </span>
            </th>
            <th class="th text-left px-3 py-2">Действия</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="agent in agents" :key="agent.id" class="border-t hover:bg-gray-50">
            <td class="px-3 py-2">{{ agent.id }}</td>
            <td class="px-3 py-2">{{ agent.repFullName }}</td>
            <td class="px-3 py-2">
              <div>{{ agent.repEmail }}</div>
              <div>+7{{ agent.repPhoneNumber }}</div>
            </td>
            <td class="px-3 py-2">
              <div>{{ agent.shortName }}</div>
              <div class="text-xs text-gray-500">ИНН: {{ agent.inn }}</div>
            </td>
            <td class="px-3 py-2">{{ formatDate(agent.ogrnDateOfAssignment) }}</td>
            <td class="px-3 py-2">
              <div v-for="bank in agent.banks" :key="bank.id">
                {{ bank.shortName }}
              </div>
            </td>
            <td class="px-3 py-2 text-center">
              <input type="checkbox" class="accent-blue-600" :checked="agent.important" disabled />
            </td>
            <td class="px-3 py-2 space-x-2">
              <button @click="editAgent(agent)" class="text-blue-600 hover:underline">Ред.</button>
              <button @click="deleteAgent(agent.id)" class="text-red-600 hover:underline">Удалить</button>
            </td>
          </tr>
        </tbody>
      </table>
    </section>

    <!-- Пагинация -->
    <section class="flex justify-between items-center mb-6">
      <div>
        <label class="text-sm">На странице:</label>
        <select v-model="filters.pageSize" @change="applyFilter" class="border rounded-xl px-2 py-1 ml-2">
          <option v-for="n in [10,25,50]" :key="n" :value="n">{{ n }}</option>
        </select>
      </div>
      <div class="flex gap-2">
        <button @click="changePage(filters.pageNumber-1)" :disabled="filters.pageNumber===1" class="px-2 py-1 bg-gray-200 rounded-xl disabled:opacity-50">◀</button>
        <span class="text-sm">Стр. {{ filters.pageNumber }} / {{ totalPages }}</span>
        <button @click="changePage(filters.pageNumber+1)" :disabled="filters.pageNumber>=totalPages" class="px-2 py-1 bg-gray-200 rounded-xl disabled:opacity-50">▶</button>
      </div>
    </section>

    <!-- Модальное окно -->
    <AgentModal v-if="showModal" :agent="editingAgent" :isEdit="!!editingAgent" @close="closeModal" @saved="onModalSaved" />
  </div>
</template>

<script setup>
  import { reactive, ref, onMounted, computed } from 'vue'
  import { useRouter, RouterLink } from 'vue-router'
  import axios from 'axios'
  import AgentModal from './components/AgentModal.vue'

  const tabs = [
    { name: 'Компании агентов', route: '/agents' },
    { name: 'Банки партнеры', route: '/banks' },
    { name: 'Компании клиентов', route: '/clients' }
  ]

  const filters = reactive({
    inn: '',
    repPhoneNumber: '',
    repEmail: '',
    important: '',
    ogrnDateFrom: '',
    ogrnDateTo: '',
    pageNumber: 1,
    pageSize: 10,
    sortBy: 'id',
    sortDirection: 'asc'
  })

  const agents = ref([])
  const totalCount = ref(0)
  const showModal = ref(false)
  const editingAgent = ref(null)

  async function fetchAgents() {
    const { data } = await axios.get('/api/agents/search', { params: filters })
    totalCount.value = data.totalCount
    agents.value = data.items
  }

  function setSort(column) {
    if (filters.sortBy === column) {
      filters.sortDirection = filters.sortDirection === 'asc' ? 'desc' : 'asc'
    } else {
      filters.sortBy = column
      filters.sortDirection = 'asc'
    }

    fetchAgents()
  }

  function applyFilter() {
    filters.pageNumber = 1
    fetchAgents()
  }

  function changePage(page) {
    if (page < 1 || page > totalPages.value) return
    filters.pageNumber = page
    fetchAgents()
  }

  function openModal() {
    editingAgent.value = null
    showModal.value = true
  }

  function editAgent(agent) {
    editingAgent.value = agent
    showModal.value = true
  }

  async function deleteAgent(id) {
    if (!confirm('Удалить агента?')) return
    await axios.delete(`/api/agents/${id}`)
    fetchAgents()
  }

  function closeModal() {
    showModal.value = false
  }

  function onModalSaved() {
    showModal.value = false
    fetchAgents()
  }

  const totalPages = computed(() => Math.ceil(totalCount.value / filters.pageSize))

  function formatDate(d) {
    if (!d) return ''
    const dt = new Date(d)
    return dt.toLocaleDateString()
  }

  onMounted(fetchAgents)
</script>

<style>
  .app-container {
    max-width: 1400px;
    margin: auto;
    padding: 2rem;
  }

  .nav-tabs {
    display: flex;
    gap: 1rem;
  }

  .tab-item {
    position: relative;
  }

  .filter-grid {
    display: grid;
    gap: 1rem;
  }
</style>
