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
import { Badge } from "@/components/ui/badge";

const BACKEND_URL = process.env.BACKEND_URL || "http://localhost:5000";

function TH({ children }) {
    return (
        <TableHead className={"text-center"}>
            {children}
        </TableHead>
    )
}

function Cell({ children }) {
    return (
        <TableCell className={"text-center"}>
            {children}
        </TableCell>
    )
}

function ExpenseButton({ children }) {
    return (
        <Button className={"bg-identity-dillute hover:bg-identity"}>
            {children}
        </Button>
    )
}

export default async function Expenses(props) {

    let Expenses = [];
    try {
        const res = await fetch(`${BACKEND_URL}/api/Expenses`, {
            method: "GET",
            cache: 'no-store'
        });
        // if (!res.ok) throw new Error('Failed to load Expenses');
        if (res.ok)
            Expenses = await res.json();
    } catch (e) {
        console.error("Failed to load Expenses,", e);
        Expenses = [];
    }
    return (
        <main className="flex flex-col h-min-full w-full p-4 lg:p-0 ">
            <div className="container mx-auto my-12">
                <h1 className="text-5xl font-bold mb-8">Expenses</h1>
                <p>Generate Expenses with just a click of a button</p>
            </div>

            <div className="container mx-auto m-4">
                <Link className="bg-identity object flex p-2 text-background rounded-sm w-fit" href={"expenses/new"}>Add New Expense</Link>
            </div>


            {

                Expenses.length > 0 ?
                    (<>
                        <Table className={"container mx-auto border-2 border-identity-dillute/20 rounded-xl "}>
                            <TableHeader >
                                <TableRow className={"bg-accent rounded-xl"}>
                                    <TH>Customers</TH>
                                    <TH>Status</TH>
                                    <TH>Order Date

                                    </TH>
                                    <TH>Due Date</TH>
                                    <TH>Generate Expense</TH>
                                    <TH>Send an email</TH>
                                    <TH>Receipt</TH>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {
                                    Expenses.map((Expense, index) => (
                                        <TableRow key={index} >
                                            <Cell>
                                                {Expense.UserId}
                                            </Cell>
                                            <Cell>
                                                {Expense.Status}
                                            </Cell>
                                            <Cell>
                                                {Expense.OrderDate}
                                            </Cell>
                                            <Cell>
                                                {Expense.DueDate}
                                            </Cell>
                                            <Cell>
                                                <ExpenseButton>
                                                    Expense
                                                </ExpenseButton>
                                            </Cell>
                                            <Cell>
                                                <ExpenseButton>send email</ExpenseButton>
                                            </Cell>
                                            <Cell>
                                                <Link
                                                    href={"/Expenses"}>
                                                    {""}
                                                    {/* Later to be changed */}
                                                </Link>
                                            </Cell>
                                        </TableRow>
                                    ))
                                }


                            </TableBody>

                        </Table>
                    </>)
                    :
                    (<>
                        <Badge variant={"destructive"} className={"mx-auto"}>Failed to load Expenses</Badge>
                        <Table className={"container mx-auto border-2 border-identity-dillute/20 rounded-xl "}>
                            <TableHeader >
                                <TableRow className={"bg-accent rounded-xl"}>
                                    <TH>Customers</TH>
                                    <TH>Status</TH>
                                    <TH>Order Date

                                    </TH>
                                    <TH>Due Date</TH>
                                    <TH>Generate Expense</TH>
                                    <TH>Send an email</TH>
                                    <TH>Receipt</TH>
                                </TableRow>
                            </TableHeader>
                            <TableBody></TableBody>
                        </Table>
                    </>)
            }

        </main>
    )
}

const dummyExpenses = [
    {
        ExpenseId: "INV0001",
        ExpenseId: "CS0001",
        UserId: "US0001",
        Status: "Cancelled",
        OrderDate: "DD/MM/YYYY",
        DueDate: "DD/MM/YYYY",
        TotalAmount: 1000,
        CreatedAt: "DD/MM/YYYY HH:MM:SS",

        ExpensePdfFileName: "",
        EmailMessageId: "",
        EmaiThreadId: "",
        ExpenseEmailSentAt: "",

        Expense: {},
        User: {},
        Items: {},
        Receipts: {}
    },
];