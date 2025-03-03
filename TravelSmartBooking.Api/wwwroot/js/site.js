document.addEventListener('DOMContentLoaded', function () {
    fetchPackages();

    document.getElementById('createPackageForm').addEventListener('submit', function (event) {
        event.preventDefault();
        createPackage();
    });

    const viewBookingsModal = document.getElementById('viewBookingsModal');

    if (viewBookingsModal) {
        viewBookingsModal.addEventListener('show.bs.modal', event => {
          fetchBookings();
        });
    } else {
        console.error("viewBookingsModal element not found!");
    }
});

async function fetchPackages() {
    try {
        const response = await fetch('/api/packages');

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const packages = await response.json();
        displayPackages(packages);
    } catch (error) {
        console.error('Error fetching packages:', error);
        displayErrorMessage('Failed to load packages. Please try again later.');
    }
}

function displayPackages(packages) {
    const packagesList = document.getElementById('packageContainer');
    packagesList.innerHTML = '';

    if (packages.length === 0) {
        packagesList.innerHTML = '<p>No packages available at the moment.</p>';
        return;
    }

    packages.forEach(package => {
        const packageCard = createPackageCard(package);
        packagesList.appendChild(packageCard);
    });
}

function createPackageCard(package) {
    const colDiv = document.createElement('div');
    colDiv.className = 'col-md-4 mb-3';

    const cardDiv = document.createElement('div');
    cardDiv.className = 'card package-card h-100';

    const cardBody = document.createElement('div');
    cardBody.className = 'card-body d-flex flex-column';

    const title = document.createElement('h5');
    title.className = 'card-title';
    title.textContent = package.name;

    const description = document.createElement('p');
    description.className = 'card-text';
    description.textContent = package.description;

    const price = document.createElement('p');
    price.className = 'card-text';
    price.textContent = `Price: $${package.price}`;

    const availability = document.createElement('p');
    availability.className = 'card-text';
    availability.textContent = `Availability: ${package.availability}`;

    const buttonGroup = document.createElement('div');
    buttonGroup.className = 'mt-auto';

    const viewDetailsButton = document.createElement('button');
    viewDetailsButton.className = 'btn btn-primary me-2';
    viewDetailsButton.textContent = 'View Details';
    viewDetailsButton.addEventListener('click', () => displayPackageDetails(package));

    const editButton = document.createElement('button');
    editButton.className = 'btn btn-warning me-2';
    editButton.innerHTML = '<i class="bi bi-pencil-square"></i> Edit';
    editButton.addEventListener('click', () => editPackage(package));

    const deleteButton = document.createElement('button');
    deleteButton.className = 'btn btn-danger';
    deleteButton.innerHTML = '<i class="bi bi-trash"></i> Delete';
    deleteButton.addEventListener('click', () => deletePackage(package.id));

    buttonGroup.appendChild(viewDetailsButton);
    buttonGroup.appendChild(editButton);
    buttonGroup.appendChild(deleteButton);

    cardBody.appendChild(title);
    cardBody.appendChild(description);
    cardBody.appendChild(price);
    cardBody.appendChild(availability);
    cardBody.appendChild(buttonGroup);

    cardDiv.appendChild(cardBody);
    colDiv.appendChild(cardDiv);

    return colDiv;
}

async function bookPackage(package) {
    const formHtml = `
        <form id="bookingForm">
            <div class="mb-3">
                <label for="customerName" class="form-label">Name:</label>
                <input type="text" class="form-control" id="customerName" required>
            </div>
            <div class="mb-3">
                <label for="customerEmail" class="form-label">Email:</label>
                <input type="email" class="form-control" id="customerEmail" required>
            </div>
             <div class="mb-3">
                <label for="customerDetails" class="form-label">Details:</label>
                <input type="text" class="form-control" id="customerDetails" required>
            </div>
            <div class="mb-3">
                <label class="form-label">Confirmation Method:</label><br>
                <div class="form-check form-check-inline">
                    <input class="form-check-input" type="radio" name="confirmationMethod" id="emailConfirmation" value="email" checked>
                    <label class="form-check-label" for="emailConfirmation">Email</label>
                </div>
                <div class="form-check form-check-inline">
                    <input class="form-check-input" type="radio" name="confirmationMethod" id="smsConfirmation" value="sms">
                    <label class="form-check-label" for="smsConfirmation">SMS</label>
                </div>
            </div>
            <button type="submit" class="btn btn-primary">Confirm Booking</button>
        </form>
    `;

    const packageModal = new bootstrap.Modal(document.getElementById('packageModal'));
    const modalBody = document.querySelector('#packageModal .modal-body');
    modalBody.innerHTML = formHtml;
    const modalTitle = document.querySelector('#packageModal .modal-title');
    modalTitle.textContent = `Book ${package.name}`;

    const bookingForm = document.getElementById('bookingForm');
    if(bookingForm){
      bookingForm.addEventListener('submit', async (event) => {
        event.preventDefault();

        const customerName = document.getElementById('customerName').value;
        const customerEmail = document.getElementById('customerEmail').value;
        const customerDetails = document.getElementById('customerDetails').value;
        const confirmationMethod = document.querySelector('input[name="confirmationMethod"]:checked').value; // Get selected method

        const bookingData = {
            customerName: customerName,
            customerEmail: customerEmail,
            customerDetails: customerDetails,
            confirmationMethod: confirmationMethod,
        };

        try {
            const response = await fetch(`/api/bookings/${package.id}/book`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(bookingData),
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            packageModal.hide();
            const successModal = new bootstrap.Modal(document.getElementById('successModal'));
            const successMessage = document.getElementById('successMessage');
            if (confirmationMethod === 'email') {
                successMessage.textContent = 'Your booking has been confirmed! An email has been sent.';
            } else if (confirmationMethod === 'sms') {
                successMessage.textContent = 'Your booking has been confirmed! An SMS has been sent.';
            } else {
                successMessage.textContent = 'Your booking has been confirmed!'; // Fallback
            }
            successModal.show();


            fetchPackages();

            setTimeout(refreshPage, 3000);

        } catch (error) {
            console.error('Error booking package:', error);
            alert('Failed to book package.');

            setTimeout(refreshPage, 3000);
        }
    });
    }

    const modalFooter = document.querySelector('#packageModal .modal-footer');
    modalFooter.innerHTML = `
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
    `;
    packageModal.show();
}

function refreshPage() {
    location.reload();
}

async function displayPackageDetails(package) {
    const modalTitle = document.querySelector('#packageModal .modal-title');
    const modalBody = document.querySelector('#packageModal .modal-body');

    modalTitle.textContent = `Package Details`;
    modalBody.innerHTML = `
        <p><strong>Name:</strong> ${package.name}</p>
        <p><strong>Description:</strong> ${package.description}</p>
        <p><strong>Price:</strong> $${package.price}</p>
        <p><strong>Availability:</strong> ${package.availability}</p>
        <p><strong>Prerequisites:</strong> ${package.prerequisites}</p>
    `;

    const modalFooter = document.querySelector('#packageModal .modal-footer');
    modalFooter.innerHTML = `
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
        <button type="button" class="btn btn-primary" onclick='bookPackage(${JSON.stringify(package).replaceAll("'", "&apos;")})'>Book Now</button>
    `;

    const packageModal = new bootstrap.Modal(document.getElementById('packageModal'));
    packageModal.show();

}

function displayErrorMessage(message) {
    const packagesList = document.getElementById('packageContainer');
    packagesList.innerHTML = `<p class="text-danger">${message}</p>`;
}

  async function createPackage() {
    const name = document.getElementById('packageName').value;
    const description = document.getElementById('packageDescription').value;
    const price = parseFloat(document.getElementById('packagePrice').value);
    const availability = parseInt(document.getElementById('packageAvailability').value);
    const prerequisites = document.getElementById('packagePrerequisites').value;

    const newPackage = {
        name: name,
        description: description,
        price: price,
        availability: availability,
        prerequisites: prerequisites
    };

    try {
        const response = await fetch('/api/packages', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(newPackage)
        });

        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }

        fetchPackages();

        const createPackageModal = bootstrap.Modal.getInstance(document.getElementById('createPackageModal'));
        createPackageModal.hide();
        document.getElementById('createPackageForm').reset();

        alert('Package Created Successfully');

    } catch (error) {
        console.error("Error creating package:", error);
        alert('Failed to create package. Please try again.');
    }
}

function editPackage(package) {
    document.getElementById('packageName').value = package.name;
    document.getElementById('packageDescription').value = package.description;
    document.getElementById('packagePrice').value = package.price;
    document.getElementById('packageAvailability').value = package.availability;
    document.getElementById('packagePrerequisites').value = package.prerequisites;

    document.getElementById('createPackageModalLabel').textContent = 'Edit Package';
    const submitButton = document.querySelector('#createPackageModal button[type="submit"]');
    submitButton.textContent = 'Save Changes';

    const newSubmitButton = submitButton.cloneNode(true);
    submitButton.parentNode.replaceChild(newSubmitButton, submitButton);

    newSubmitButton.addEventListener('click', function updatePackageHandler(event) {

        event.preventDefault();

        const updatedPackage = {
            id: package.id,
            name: document.getElementById('packageName').value,
            description: document.getElementById('packageDescription').value,
            price: parseFloat(document.getElementById('packagePrice').value),
            availability: parseInt(document.getElementById('packageAvailability').value),
            prerequisites: document.getElementById('packagePrerequisites').value
        };

        updateExistingPackage(updatedPackage);

        newSubmitButton.removeEventListener('click', updatePackageHandler);

        document.getElementById('createPackageModalLabel').textContent = 'Create New Package';
        const submitButtonReset = document.querySelector('#createPackageModal button[type="submit"]');
        submitButtonReset.textContent = 'Create Package';
    });

    const createPackageModal = new bootstrap.Modal(document.getElementById('createPackageModal'));
    createPackageModal.show();

}

async function updateExistingPackage(updatedPackage) {
    try {
        const response = await fetch(`/api/packages/${updatedPackage.id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(updatedPackage)
        });

        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }

        fetchPackages();

        const createPackageModal = bootstrap.Modal.getInstance(document.getElementById('createPackageModal'));
        createPackageModal.hide();
        document.getElementById('createPackageForm').reset();

        alert('Package Updated Successfully');

    } catch (error) {
        console.error("Error updating package:", error);
        alert('Failed to update package. Please try again.');
    }
}

async function deletePackage(packageId) {
    if (confirm('Are you sure you want to delete this package?')) {
        try {
            const response = await fetch(`/api/packages/${packageId}`, {
                method: 'DELETE',
            });

            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }

            fetchPackages();
            alert('Package Deleted Successfully');

        } catch (error) {
            console.error("Error deleting package:", error);
            alert('Failed to delete package. Please try again.');
        }
    }
}

async function fetchBookings() {
    try {
        const response = await fetch('/api/bookings');
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        const bookings = await response.json();
        displayBookings(bookings);
    } catch (error) {
        console.error('Error fetching bookings:', error);
        displayBookingErrorMessage('Failed to load bookings.');
    }
}

function displayBookings(bookings) {
    const bookingsList = document.getElementById('bookingsList');
    bookingsList.innerHTML = '';

    if (bookings.length === 0) {
        bookingsList.innerHTML = '<p>No bookings found.</p>';
        return;
    }

    const container = document.createElement('div'); // Use a div for more flexible layout

    bookings.forEach(booking => {
        const bookingCard = document.createElement('div');
        bookingCard.className = 'card mb-3'; // Bootstrap card styling, 'mb-3' for margin-bottom

        const cardBody = document.createElement('div');
        cardBody.className = 'card-body';

        // --- Format the date ---
        const formattedDate = new Date(booking.bookingDate).toLocaleString('en-US', {
            year: 'numeric',
            month: 'long',
            day: 'numeric',
            hour: 'numeric',
            minute: 'numeric',
            second: 'numeric',
            hour12: true // Use 12-hour format (AM/PM)
        });

        // --- Create individual elements for better styling and structure ---
        const bookingIdElement = document.createElement('p');
        bookingIdElement.className = 'card-text'; // Bootstrap card text style
        bookingIdElement.innerHTML = `<strong>Booking ID:</strong> ${booking.id}`;

        const packageIdElement = document.createElement('p');
        packageIdElement.className = 'card-text';
        packageIdElement.innerHTML = `<strong>Package ID:</strong> ${booking.packageId}`;

        const dateElement = document.createElement('p');
        dateElement.className = 'card-text';
        dateElement.innerHTML = `<strong>Booking Date:</strong> ${formattedDate}`;

        const detailsElement = document.createElement('p');
        detailsElement.className = 'card-text';

        // --- Handle potentially long customer details ---
        // Split the details string (assuming it's comma-separated)
        const detailsParts = booking.customerDetails.split(', ');
        let formattedDetails = '';
        detailsParts.forEach(part => {
          formattedDetails += `<p>${part}</p>`;
        });

        detailsElement.innerHTML = `<strong>Customer Details:</strong> ${formattedDetails}`;



        // --- Assemble the card ---
        cardBody.appendChild(bookingIdElement);
        cardBody.appendChild(packageIdElement);
        cardBody.appendChild(dateElement);
        cardBody.appendChild(detailsElement);

        bookingCard.appendChild(cardBody);
        container.appendChild(bookingCard);
    });

    bookingsList.appendChild(container);
}

function displayBookingErrorMessage(message) {
    const bookingsList = document.getElementById('bookingsList');
    bookingsList.innerHTML = `<p class="text-danger">${message}</p>`;
}