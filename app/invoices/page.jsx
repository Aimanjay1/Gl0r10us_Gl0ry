
import { Button } from "@/components/ui/button";
import {
    Table,
    TableBody,
    TableCaption,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from "@/components/ui/table"
import Link from "next/link";

function Cell({ children }) {
    return (
        <TableCell className={"text-center"}>
            {children}
        </TableCell>
    )
}

function InvoiceButton({ children }) {
    return (
        <Button className={"bg-identity-dillute hover:bg-identity"}>
            {children}
        </Button>
    )
}

export default function Invoice(props) {


    return (
        <main className="flex flex-col h-min-full w-full">
            <div className="container mx-auto my-12">
                <h1 className="text-5xl font-bold mb-8">Invoices</h1>
                <p>Generate invoices with just a click of a button</p>
            </div>


            <Table className={"container mx-auto"}>
                <TableHeader>
                    <TableRow className={"bg-accent"}>
                        <TableHead>Customers</TableHead>
                        <TableHead>Status</TableHead>
                        <TableHead>Order Date</TableHead>
                        <TableHead>Due Date</TableHead>
                        <TableHead>Generate Invoice</TableHead>
                        <TableHead>Send an email</TableHead>
                        <TableHead>Receipt</TableHead>
                    </TableRow>
                </TableHeader>

                <TableBody>{
                    dummyCustomers.map((value, index) => (
                        <TableRow key={index} >
                            <Cell>
                                {value.Name}
                            </Cell>
                            <Cell>
                                {value.InvoiceStatus}
                            </Cell>
                            <Cell>
                                {value.OrderDate}
                            </Cell>
                            <Cell>
                                {value.DueDate}
                            </Cell>
                            <Cell>
                                <InvoiceButton>
                                    Invoice
                                </InvoiceButton>
                            </Cell>
                            <Cell>
                                <InvoiceButton>send email</InvoiceButton>
                            </Cell>
                            <Cell>
                                <Link
                                    href={value.Receipt.Link}>
                                    {value.Receipt.ReceiptId}
                                </Link>
                            </Cell>
                        </TableRow>
                    ))
                }

                </TableBody>

            </Table>


        </main>
    )
}

const dummyCustomers = [
    {
        CustomerId: "CS0001",
        Name: "Joanne Lee",
        InvoiceStatus: "Cancelled",
        OrderDate: "DD/MM/YYYY",
        DueDate: "DD/MM/YYYY",
        InvoiceLink: "https://answerspace.mimoimio.com",
        Receipt: {
            Link: "https://answerspace.mimoimio.com",
            ReceiptId: "INV0001.jpeg"
        }
    },
    {
        CustomerId: "CS0001",
        Name: "Joanne Lee",
        InvoiceStatus: "Cancelled",
        OrderDate: "DD/MM/YYYY",
        DueDate: "DD/MM/YYYY",
        InvoiceLink: "https://answerspace.mimoimio.com",
        Receipt: {
            Link: "https://answerspace.mimoimio.com",
            ReceiptId: "INV0001.jpeg"
        }
    },
    {
        CustomerId: "CS0001",
        Name: "Joanne Lee",
        InvoiceStatus: "Cancelled",
        OrderDate: "DD/MM/YYYY",
        DueDate: "DD/MM/YYYY",
        InvoiceLink: "https://answerspace.mimoimio.com",
        Receipt: {
            Link: "https://answerspace.mimoimio.com",
            ReceiptId: "INV0001.jpeg"
        }
    },
    {
        CustomerId: "CS0001",
        Name: "Joanne Lee",
        InvoiceStatus: "Cancelled",
        OrderDate: "DD/MM/YYYY",
        DueDate: "DD/MM/YYYY",
        InvoiceLink: "https://answerspace.mimoimio.com",
        Receipt: {
            Link: "https://answerspace.mimoimio.com",
            ReceiptId: "INV0001.jpeg"
        }
    },
    {
        CustomerId: "CS0001",
        Name: "Joanne Lee",
        InvoiceStatus: "Cancelled",
        OrderDate: "DD/MM/YYYY",
        DueDate: "DD/MM/YYYY",
        InvoiceLink: "https://answerspace.mimoimio.com",
        Receipt: {
            Link: "https://answerspace.mimoimio.com",
            ReceiptId: "INV0001.jpeg"
        }
    },
    {
        CustomerId: "CS0001",
        Name: "Joanne Lee",
        InvoiceStatus: "Cancelled",
        OrderDate: "DD/MM/YYYY",
        DueDate: "DD/MM/YYYY",
        InvoiceLink: "https://answerspace.mimoimio.com",
        Receipt: {
            Link: "https://answerspace.mimoimio.com",
            ReceiptId: "INV0001.jpeg"
        }
    },
    {
        CustomerId: "CS0001",
        Name: "Joanne Lee",
        InvoiceStatus: "Cancelled",
        OrderDate: "DD/MM/YYYY",
        DueDate: "DD/MM/YYYY",
        InvoiceLink: "https://answerspace.mimoimio.com",
        Receipt: {
            Link: "https://answerspace.mimoimio.com",
            ReceiptId: "INV0001.jpeg"
        }
    },
];