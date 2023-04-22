
function createCveTable(data) {
    table = $('#cve_table').DataTable({
        data: data,
        columns: [
            { data: 'cveID' },
            { data: 'baseScore' },
            { data: 'baseSeverity' },
            { data: 'published' },
            { data: 'vendor' },
            { data: 'application' },
            { data: 'version' },
        ],
    });
}



