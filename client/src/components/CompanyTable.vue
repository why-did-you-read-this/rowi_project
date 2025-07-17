<template>
  <div>
    <!-- Фильтры -->
    <section class="filter-section bg-white p-4 rounded-xl shadow mb-4">
      <h3 class="text-lg font-semibold mb-2">Настройка фильтра</h3>
      <div class="filter-grid grid grid-cols-1 md:grid-cols-3 lg:grid-cols-3 gap-4 mb-4">
        <div>
          <label class="block mb-1 text-sm font-medium">ИНН</label>
          <input v-if="filters.inn" v-model="filterValues.inn" type="text" placeholder="Введите ИНН" class="border rounded-xl px-3 py-2 w-full" />
        </div>
        <div>
          <label class="block mb-1 text-sm font-medium">Телефон</label>
          <input v-if="filters.phone" v-model="filterValues.phone" type="text" placeholder="Введите телефон" class="border rounded-xl px-3 py-2 w-full" />
        </div>
        <div>
          <label class="block mb-1 text-sm font-medium">Email</label>
          <input v-if="filters.email" v-model="filterValues.email" type="text" placeholder="Введите email" class="border rounded-xl px-3 py-2 w-full" />
        </div>
        <div>
          <label class="block mb-1 text-sm font-medium">ОГРН от</label>
          <input v-model="filterValues.ogrnDateFrom" type="date" class="border rounded-xl px-3 py-2 w-full" />
        </div>
        <div>
          <label class="block mb-1 text-sm font-medium">ОГРН до</label>
          <input v-model="filterValues.ogrnDateTo" type="date" class="border rounded-xl px-3 py-2 w-full" />
        </div>

        <div v-if="showImportant" class="flex items-center mt-6 gap-2">
          <input type="checkbox" v-model="filterValues.important" />
          <label>Только приоритетные</label>
        </div>
      </div>
      <button @click="fetchData"
              class="flex add-btn items-center bg-blue-600 text-white px-4 py-2 gap-2 rounded-xl hover:bg-blue-700 mt-4">
        <Funnel class="w-5 h-5"/>
        <span>Применить фильтрацию</span>
      </button>
    </section>

    <!-- Таблица -->
    <section class="bg-white rounded-xl shadow p-4 mb-4 relative">
      <div v-if="isLoading" class="spinner">
        <div class="spinner-circle"></div>
        <Loader class="w-10 h-10 animate-spin text-blue-600" />
      </div>
      <div class="flex justify-between items-center mb-4">
        <h2 class="text-xl font-semibold">Список агентов</h2>
        <button class="flex add-btn items-center bg-green-600 text-white px-4 py-2 gap-2 rounded-xl hover:bg-green-700"
                @click="openModal">
          <PlusCircle class="w-5 h-5" />
          <span>Добавить</span>
        </button>
      </div>
      <table class="min-w-full text-sm table-auto">
        <!--<colgroup>
          <col style="width: 6%" />
          <col style="width: 20%" />
          <col style="width: 18%" />
          <col style="width: 18%" />
          <col style="width: 18%" />
          <col style="width: 14%" />
          <col style="width: 8%" />
          <col style="width: 8%" />
        </colgroup>-->
        <thead class="bg-gray-100">
          <tr>
            <th v-for="col in columns" :key="col.key" class="th text-left px-3 py-2">
              <button @click="sortableFields.includes(col.key) && changeSort(col.key)"
                      class="flex items-center gap-1 group disabled:cursor-default">
                {{ col.label }}
                <template v-if="sortableFields.includes(col.key)">
                  <ArrowUp v-if="sortBy === col.key && sortDir === 'asc'"
                           class="w-4 h-4 text-blue-600" />
                  <ArrowDown v-else-if="sortBy === col.key && sortDir === 'desc'"
                             class="w-4 h-4 text-blue-600" />
                  <ArrowUpDown v-else
                           class="w-4 h-4 text-gray-400 group-hover:text-blue-600" />
                </template>
              </button>
            </th>
            <th v-if="canEdit" class="th text-left px-3 py-2">Действия</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in data" :key="item.id" class="border-t hover:bg-gray-50">
            <td v-for="col in columns" :key="col.key" class="px-3 py-2">
              {{ getValue(item, col.key) }}
            </td>
            <td v-if="canEdit" class="px-4 py-2 flex gap-2">
              <button @click="$emit('edit', item)" class="text-blue-600 flex items-center">
                <Edit2 class="w-5 h-5" />
              </button>
              <button @click="$emit('delete', item)" class="text-red-600 flex items-center">
                <Trash2 class="w-5 h-5" />
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </section>

    <!-- Пагинация -->
    <div class="mt-4 flex items-center justify-between mb-5">
      <label class="text-sm">
        На странице:
        <select v-model="pageSize" @change="fetchData" class="border rounded px-2 py-1 ml-2">
          <option v-for="opt in [10, 20, 50]" :value="opt">{{ opt }}</option>
        </select>
      </label>

      <div class="flex gap-2">
        <button @click="prevPage" :disabled="page === 1"
                class="px-1 py-1 border rounded hover:bg-gray-200">
          <ChevronLeft class="w-5 h-5"/>
        </button>
        <span>Стр. {{ page }} / {{ maxPage }}</span>
        <button @click="nextPage" :disabled="page === maxPage"
                class="px-1 py-1 border rounded hover:bg-gray-200">
          <ChevronRight class="w-5 h-5"/>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted, watch, computed, reactive } from 'vue'
  import {
    ArrowUp, ArrowDown, ArrowUpDown, Edit2, Trash2, PlusCircle, Loader, Funnel, ChevronRight, ChevronLeft
  } from 'lucide-vue-next'
  import axios from 'axios'

  const sortableFields = ['id', 'ogrnDateOfAssignment', 'important']

  const props = defineProps({
    title: String,
    fetchUrl: String,
    canEdit: Boolean,
    showImportant: Boolean,
    columns: {
      type: Array,
      default: () => []
    },
    filters: {
      type: Object,
      default: () => ({})
    }
  })

  const emit = defineEmits(['add', 'edit', 'delete'])

  const data = ref([])
  const page = ref(1)
  const pageSize = ref(10)
  const total = ref(0)
  const sortBy = ref('id')
  const sortDir = ref('asc')
  const loading = ref(false)

  const filterValues = ref({
    inn: '',
    phone: '',
    email: '',
    ogrnDateFrom: null,
    ogrnDateTo: null,
    important: false
  })

  const maxPage = computed(() => Math.ceil(total.value / pageSize.value))

  async function fetchData() {
    loading.value = true
    try {
      const params = {
        PageNumber: page.value,
        PageSize: pageSize.value,
        SortBy: sortBy.value,
        SortDirection: sortDir.value,
        Inn: filterValues.value.inn,
        RepPhoneNumber: filterValues.value.phone,
        RepEmail: filterValues.value.email,
        OgrnDateFrom: filterValues.value.ogrnDateFrom,
        OgrnDateTo: filterValues.value.ogrnDateTo,
        Important: props.showImportant ? filterValues.value.important : null
      }
      const res = await axios.get(props.fetchUrl, { params })
      data.value = res.data.items
      total.value = res.data.totalCount
    } finally {
      loading.value = false
    }
  }

  function changeSort(key) {
    if (sortBy.value === key) {
      sortDir.value = sortDir.value === 'asc' ? 'desc' : 'asc'
    } else {
      sortBy.value = key
      sortDir.value = 'asc'
    }
    fetchData()
  }

  function prevPage() {
    if (page.value > 1) {
      page.value--
      fetchData()
    }
  }

  function nextPage() {
    if (page.value < maxPage.value) {
      page.value++
      fetchData()
    }
  }

  function getValue(item, key) {
    if (key === 'contacts') return `${item.repEmail}\n${item.repPhoneNumber}`
    if (key === 'company') return `${item.shortName} \nИНН: ${item.inn}`
    if (key === 'banks') return item.banks?.map(b => b.shortName).join(', ')
    if (key === 'ogrnDateOfAssignment') return new Date(item.ogrnDateOfAssignment).toLocaleDateString()
    return item[key]
  }

  function openModal() {
    emit('add')
  }

  function editAgent(agent) {
    emit('edit')
  }

  async function deleteAgent(id) {
    emet('delete')
  }

  function closeModal() {
    showModal.value = false
  }

  function onModalSaved() {
    showModal.value = false
    fetchAgents()
  }

  onMounted(fetchData)
</script>
