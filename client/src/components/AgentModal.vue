<template>
  <div class="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
    <div class="bg-white rounded-2xl p-6 w-full max-w-lg shadow-lg relative">
      <button class="absolute top-4 right-4 text-gray-400 hover:text-gray-600" @click="$emit('close')">
        ✕
      </button>
      <h2 class="text-xl font-semibold mb-4">
        {{ isEdit ? 'Редактировать агента' : 'Создать агента' }}
      </h2>
      <form @submit.prevent="onSubmit">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div class="col-span-2">
            <label class="block text-sm font-medium mb-1">Краткое наименование</label>
            <input v-model="form.shortName" type="text" class="w-full border rounded-md px-3 py-2" required />
          </div>
          <div class="col-span-2">
            <label class="block text-sm font-medium mb-1">Полное наименование</label>
            <input v-model="form.fullName" type="text" class="w-full border rounded-md px-3 py-2" required />
          </div>
          <div>
            <label class="block text-sm font-medium mb-1">ИНН</label>
            <input v-model="form.inn" type="text" class="w-full border rounded-md px-3 py-2" required />
          </div>
          <div>
            <label class="block text-sm font-medium mb-1">КПП</label>
            <input v-model="form.kpp" type="text" class="w-full border rounded-md px-3 py-2" required />
          </div>
          <div>
            <label class="block text-sm font-medium mb-1">ОГРН</label>
            <input v-model="form.ogrn" type="text" class="w-full border rounded-md px-3 py-2" required />
          </div>
          <div>
            <label class="block text-sm font-medium mb-1">Дата присвоения ОГРН</label>
            <input v-model="form.ogrnDateOfAssignment" type="date" class="w-full border rounded-md px-3 py-2" required />
          </div>
          <div class="col-span-2">
            <label class="block text-sm font-medium mb-1">ФИО представителя</label>
            <input v-model="form.repFullName" type="text" class="w-full border rounded-md px-3 py-2" required />
          </div>
          <div>
            <label class="block text-sm font-medium mb-1">Email</label>
            <input v-model="form.repEmail" type="email" class="w-full border rounded-md px-3 py-2" required />
          </div>
          <div>
            <label class="block text-sm font-medium mb-1">Телефон</label>
            <input v-model="form.repPhoneNumber" type="text" class="w-full border rounded-md px-3 py-2" required />
          </div>
          <div class="flex items-center mt-2">
            <input :checked="form.important === true" @change="form.important = $event.target.checked ? true : null" v-model="form.important" type="checkbox" id="important" class="mr-2" />
            <label for="important" class="text-sm">Приоритетный</label>
          </div>

          <div class="col-span-2">
            <label class="block text-sm font-medium mb-1">Банки агента:</label>
            <ul>
              <li v-for="bank in form.banks" :key="bank.id" class="flex items-center justify-between border p-2 rounded mb-1">
                <span>{{ bank.shortName }}</span>
                <button type="button" @click="removeBank(bank.id)" class="text-red-600 hover:underline">Удалить</button>
              </li>
            </ul>
          </div>

          <div class="col-span-2">
            <label class="block font-medium mb-1">Добавить банк:</label>
            <input v-model="bankSearch"
                   @input="searchBanks"
                   type="text"
                   class="w-full border rounded-md px-3 py-2"
                   placeholder="Начните вводить название банка..." />

            <ul v-if="bankResults.length" class="border rounded mt-2 max-h-48 overflow-auto">
              <li v-for="bank in bankResults"
                  :key="bank.id"
                  class="p-2 hover:bg-gray-100 cursor-pointer"
                  @click="addBank(bank)">
                {{ bank.shortName }}
              </li>
            </ul>
          </div>
        </div>

        <div class="mt-6 flex justify-end gap-2">
          <button type="button"
                  class="px-4 py-2 bg-gray-200 hover:bg-gray-300 rounded-md"
                  @click="$emit('close')">
            Отмена
          </button>
          <button type="submit"
                  class="px-4 py-2 bg-blue-600 text-white hover:bg-blue-700 rounded-md">
            {{ isEdit ? 'Сохранить' : 'Создать' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted, watch } from 'vue'
  import axios from 'axios'

  const props = defineProps({
    agent: Object,
    isEdit: Boolean
  })
  const emit = defineEmits(['close', 'saved'])

  const form = ref({
    shortName: '',
    fullName: '',
    inn: '',
    kpp: '',
    ogrn: '',
    ogrnDateOfAssignment: '',
    repFullName: '',
    repEmail: '',
    repPhoneNumber: '',
    important: false,
    banks: []
  })

  const bankSearch = ref('')
  const bankResults = ref([])

  async function searchBanks() {
    if (bankSearch.value.length < 3) {
      bankResults.value = []
      return
    }
    const res = await axios.get(`/api/banks/search?name=${bankSearch.value}`)
    bankResults.value = res.data
  }

  function addBank(bank) {
    if (!form.value.banks.some(b => b.id === bank.id)) {
      form.value.banks.push(bank)
    }
    bankSearch.value = ''
    bankResults.value = []
  }

  function removeBank(id) {
    form.value.banks = form.value.banks.filter(b => b.id !== id)
  }

  watch(
    () => props.agent,
    (agent) => {
      if (agent) {
        form.value = {
          shortName: agent.shortName,
          fullName: agent.fullName,
          inn: agent.inn,
          kpp: agent.kpp,
          ogrn: agent.ogrn,
          ogrnDateOfAssignment: agent.ogrnDateOfAssignment,
          repFullName: agent.repFullName,
          repEmail: agent.repEmail,
          repPhoneNumber: agent.repPhoneNumber,
          important: agent.important,
          banks: [...agent.banks]
        }
      }
    },
    { immediate: true }
  )

  async function onSubmit() {
    const [surname = '', name = '', patronymic = null] = form.value.repFullName.trim().split(' ')

    const dto = {
      shortName: form.value.shortName,
      fullName: form.value.fullName,
      inn: form.value.inn,
      kpp: form.value.kpp,
      ogrn: form.value.ogrn,
      ogrnDateOfAssignment: form.value.ogrnDateOfAssignment,
      repSurname: surname,
      repName: name,
      repPatronymic: patronymic,
      repEmail: form.value.repEmail,
      repPhoneNumber: form.value.repPhoneNumber,
      bankIds: form.value.banks.map(b => b.id),
      important: form.value.important
    }

    if (props.isEdit) {
      await axios.put(`/api/agents/${props.agent.id}`, dto)
    } else {
      await axios.post('/api/agents', dto)
    }
    emit('saved')
  }
</script>
