

function createClientTable(data) {
    table = $('#clients_table').DataTable({
        data: data,
        columns: [
            { data: 'Name' },
            { data: 'IPAddress' },
            { data: 'OS' },
            { data: 'OSVersion' },
            { data: 'EnrollmentDate' },
            { data: 'online' },

        ],
    });
}