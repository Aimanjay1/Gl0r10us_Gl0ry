export default function InvoiceTemplatePage() {
  return (
    <>
      <div>
        <h1>Invoice Template</h1>
        <p>This is the invoice template page.</p>
      </div>
      <div className="justify-between flex items-center">
        <div>
          <h1 className="text-3xl font-bold">Company Name</h1>
          <p>Street Address</p>
          <p>City, State ZIP</p>
          <p>Phone</p>
          <p>Fax</p>
          <p>Email</p>
        </div>
        <div>
          <h1 className="text-3xl font-bold">Invoice</h1>
          <table>
            <tbody>
              <tr>
                <td>Date</td>
                <td>11/9</td>
              </tr>
              <tr>
                <td>Invoice No.</td>
                <td>123456</td>
              </tr>
              <tr>
                <td>Customer ID</td>
                <td>123</td>
              </tr>
              <tr>
                <td>Due date</td>
                <td>21/9</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
      <div>
        <table>
          <thead>
            <tr>
              <th>
                <h1>Bill To</h1>
              </th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>Client Name</td>
            </tr>
            <tr>
              <td>Company Name</td>
            </tr>
            <tr>
              <td>Street Address</td>
            </tr>
            <tr>
              <td>City, State ZIP</td>
            </tr>
            <tr>
              <td>Phone</td>
            </tr>
            <tr>
              <td>Email</td>
            </tr>
          </tbody>
        </table>
      </div>
      <div className="items-center justify-center flex flex-col">
        <table>
          <thead>
            <tr>
              <th>Description</th>
              <th>Quantity</th>
              <th>Unit Price</th>
              <th>Amount</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>Service 1</td>
              <td>3</td>
              <td>$100.00</td>
              <td>$300.00</td>
              <td>Delete</td>
            </tr>
            <tr>
              <td>Service 2</td>
              <td>1</td>
              <td>$200.00</td>
              <td>$200.00</td>
              <td>Delete</td>
            </tr>
          </tbody>
        </table>
      </div>
      <div className="flex justify-between">
        <div>
          <table>
            <thead>
              <tr>
                <th>Other Comments</th>
              </tr>
            </thead>
            <tbody>
              <tr>
                <td>Good item!</td>
              </tr>
            </tbody>
          </table>
        </div>
        <div>
          <table>
            <tbody>
              <tr>
                <th>Sub total</th>
                <td>950.00</td>
              </tr>
              <tr>
                <th>SST (6%)</th>
                <td>110.00</td>
              </tr>
              <tr>
                <th>TOTAL</th>
                <td>1,060.00</td>
              </tr>
            </tbody>
          </table>
          <p>Make all checks payable to</p>
          <p>Company Name</p>
        </div>
      </div>
      <div>
        <h1>If you have any question about this invoice, please contact</h1>
        <p>Contact Name</p>
        <p>Company Name</p>
        <p>Phone</p>
        <p>Email</p>
        <h1>Thank you for your business!</h1>
      </div>
    </>
  );
}
